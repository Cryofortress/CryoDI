# CryoDI
CryoDI - dependency injection контейнер, созданный для применения в первую очередь с Unity3D (но не обязательно). Это простой для использования инструмент, позволяющий быстро выстраивать архитектуру приложения, и при этом не отстрелить себе ногу.

**1. Как использовать CryoDI в Unity**

1.1 Создать класс, производный от UnityStarter. Переопределить в нем метод SetupContainer и поместить на сцену
```C#
using CryoDI;

public class GameMain : UnityStarter
{
    protected override void SetupContainer(CryoContainer container)
    {
        container.RegisterSingleton<GameState>(LifeTime.Global);
        container.RegisterSingleton<ITankDatabase, DemoTankDatabase>(LifeTime.Global);

        container.RegisterSingleton<CombatModel>(LifeTime.Scene);
        container.RegisterSingleton<ICombatModel>(() => container.Resolve<CombatModel>(), LifeTime.Scene);

        container.RegisterSceneObject<IWindowManager>("GlobalCanvas", "GlobalCanvas", LifeTime.Global);
        container.RegisterSceneObject<IWindowManager>("CombatCanvas", "CombatUI", LifeTime.Scene);
        container.RegisterSceneObject<Camera>("Main Camera", LifeTime.Scene);

        container.RegisterType<IPassiveAbility, Immunity>("Immunity", LifeTime.Scene);
        container.RegisterType<IPassiveAbility, CounterAttack>("Counterattack", LifeTime.Scene);
        container.RegisterType<IBot, Bot>(LifeTime.Scene);						
    }
}
```

1.2 Класс, в который нужно внедрить зависимости, унаследовать от CryoBehaviour и создать в нем набор свойств с атрибутом Dependency
```C#
using CryoDI;

public class Target : CryoBehaviour
{
    [Dependency]
    private Camera Camera { get; set; }

    // use Camera here
}
```

**2. Как все это работает?**

CryoBehaviour это специальный класс, унаследованный от MonoBehaviour. В Awake он обращается к контейнеру (который создается в UnityStarter) и просит проставить ему зависимости. Свойства, помеченные атрибутом Dependency это и есть зависимости. Контейнер находит все такие свойства в классе и проставляет им нужные значения.

Если вы не хотите наследовать свой класс от CryoBehaviour, то можно повесить на тот же game object компонент CryoBuilder. Как и CryoBehaviour, во время вызова Awake он запрашивает у контейнера все необходимые зависимости и проставляет их соседям, которые происходят от MonoBehaviour.

*CryoDI позволяет внедрять зависимости только через свойства. Не важно - public, protected или private. Все зависимости являются обязательными. Другие типы внедрения зависимостей - через конструктор и через метод не поддерживаются. Это осознанное решение со своими плюсами и минусами. Автор отдает себе отчет о его последствиях и считает, что плюсы такого подхода перевешивают минусы. Не смотря на это, CryoDI отслеживает циклические зависимости. Подробнее читайте в разделе «Циклические зависимости».*

**3. Как контейнер узнает, какие значения нужно проставить зависимостям?**

Для этого и нужно конфигурировать контейнер в методе SetupContainer. Для конфигурирования контейнера используются методы вида RegisterXXX:
```C#
// Зарегистрировать тип MyClass. При каждом запросе MyClass контейнер будет создавать новый экземпляр и возвращать его
container.RegisterType<MyClass>();

// Зарегистрировать тип MyClass как IMyInterface (MyClass должен реализовывать IMyInterface). При каждом запросе IMyInterface контейнер будет создавать новый экземпляр MyClass и возвращать его
container.RegisterType<IMyInterface, MyClass>();

// Зарегистрировать синглтон типа MyClass. При первом запросе MyClass создаст единственный экземпляр MyClass и будет возвращать его при последующих запросах
container.RegisterSingleton<MyClass>();


// Зарегистрировать синглтон типа IMyInterface (MyClass должен реализовывать IMyInterface). При первом запросе IMyInterface создаст единственный экземпляр MyClass и будет возвращать его при последующих запросах
container.RegisterSingleton<IMyInterface, MyClass>();

// Зарегистрировать инстанс класса. При всех запросах MyClass контейнер будет возвращать этот инстанс. Как синглтон, только временем и способом создания инстанса вы управляете сами
var a = new MyClass();
container.RegisterInstance(a);

// Зарегистрировать объект как экземпляр интерфейса. При всех запросах IMyInterface контейнер будет возвращать ссылку на объект a. Объект a должен реализовываать интерфейс IMyInterface
container.RegisterInstance<IMyInterface>(a);

// Зарегистрировать в контейнере объект сцены. При первом запросе MyClass конейнер найдет в сцене объект по пути “Path/To/Object”, а у него компонент MyClass. И будет возвращать на него ссылку при последующих запросах. Класс MyClass должен быть наследником UnityEngine.Component
container.RegisterSceneObject<MyClass>(“Path/To/Object”);
```

**4. Запрос объектов у контейнера в рантайме**

Использование свойств с атрибутом Dependency - самый простой способ получить ссылку на нужный объект. Но иногда необходимо получать ссылку во время выполнения. Для этого используйте метод Resolve:
```C#
MyClass a = container.Resolve<MyClass>();
```
Контейнер вернет экземпляр MyClass с проставленными в нем зависимостями. Для этого контейнер должен знать, откуда и как ему получить MyClass. Класс MyClass должен быть зарегистрирован в контейнере.

Иногда требуется проставить зависимости уже созданному объекту (например, если вы десериализоаали объект из файла). Для этого используйте метод BuildUp:
```C#
MyClass a = new MyClass();
container.BuildUp(a);
```
В этом случае контейнеру абсолютно безразлично, откуда взялся объект a. Регистрировать тип MyClass в контейнере не нужно.

Запрашивать объекты напрямую из контейнера просто и удобно, однако сам контейнер доступен только внутри метода UnityStarter.SetupContainer(). Для той же цели можно использовать интерфейс IResolver:
```C#
using CryoDI;

public class UnitFactory
{
    [Dependency]
    private IResolver<Tank> TankResolver { get; set; }

    public Tank CreateTank()
    {
        return TankResolver.Resolve();
    }
}
```
IResolver позволяет получать только объекты определенного типа. Тип определяется при описании зависимости. Когда контейнеру необходимо проставить зависимость типа IResolver<T>, он создает специальный класс, реализующий этот интерфейс и подставляет его.

Аналогично работает IBuilder. Он используется для проставления зависимостей в уже созданных объектах
```C#
using CryoDI;

public class UnitFactory
{
    [Dependency]
    private IBuilder<Tank> TankBuilder { get; set; }

    public Tank CreateTank()
    {
        var tank = new Tank();
        TankBuilder.BuildUp(tank);
        return tank;
    }
}
```


**5. Регистрация нескольких объектов одного типа**

Иногда наш код может зависеть от нескольких объектов одного и того же типа. Например, в сцене есть две камеры - для вида от первого лица и для вида из-за спины персонажа. И надо написать класс, который будет эти камеры переключать:
```C#
using CryoDI;

class CameraSwitcher
{
    [Dependency(“PrimaryCamera”)]
    private Camera FirstPersonCamera { get; set;}

    [Dependency(“SecondaryCamera”)]
    private Camera ThirdPersonCamera { get; set; }

    public void SwitchToFirstPerson()
    {
        ThirdPersonCamera.gameObject.SetActive(false);
        FirstPersonCamera.gameObject.SetActive(true);
    }


    public void SwitchToThirdPerson()
    {
        ThirdPersonCamera.gameObject.SetActive(true);
        FirstPersonCamera.gameObject.SetActive(false);
    }
}
```
Класс CameraSwitcher имеет две зависимости одного типа, которые ссылаются на разные камеры. Для этого в атрибутах Dependency указываем уникальные имена (“PrimaryCamera” и “SecondaryCamera”), и эти же имена указываем при регистрации камер в контейнере:
```C#
container.RegisterSceneObject<Camera>(“Path/To/Main camera”, “PrimaryCamera”);
container.RegisterSceneObject<Camera>(“Path/To/Other camera”, “SecondaryCamera”);
```
Второй параметр при вызове метода RegisterSceneObject это и есть имена, благодаря которым контейнер будет различать эти объекты. Имя может быть произвольным. Главное - чтобы в атрибуте зависмости Dependency использовалось то же самое имя.

Аналогично можно регистрировать в контейнере и обычные C# объекты, не только объекты сцены.

**6. Различное время жизни объектов**

При разработке игры очень важно понимать и отслеживать время жизни объектов. Чтобы помочь разработчику в этом, CryoDI позволяет задавать область видимости объекта. У объекта может быть три области видимости:

- Scene - время жизни такого объекта не превышает одной сцены
- Global - такой объект живет столько же, сколько и контейнер
- External - объект живет дольше контейнера. Контейнер никак не отслеживает

Пример:
```C#
// MainConfig - главный конфиг игры, должен существовать все время пока запущено приложение
container.RegisterSingleton<MainConfig>(LifeTime.Global);
// Танки должны жить только в игровой сцене. При выходе в главное меню они должны уничтожатсья и освобождать ресурсы
container.RegisterType<Tank>(LifeTime.Scene);
```
Используя эту информацию контейнер гарантирует что, при выходе из указанной области видимости:
- В контейнере не останется ссылок на объект
- Если объект реализует IDisposable, у него будет вызыван метод Dispose

Для объектов с областью видимости Scene удаление из контейнера и вызов Dispose произойдет при выходе из той сцены, в которой они были созданы. Это, например, позволяет создавать синглтоны, живущие только в одной сцене, удаляемые при выходе из нее и создаваемые заново при повторном входе в эту сцену.


Одна из распространенных ошибок при разработке - зависимость долгоживущего объекта от короткоживущего. Объект-долгожитель не позволяет сборщику мусора освободить память, занимаемую более короткоживущим объектом, поскольку сохраняет ссылку на него. CryoDI позволяет отлавливать подобные ситуации. Для того, чтобы задать, как CryoDI должен реагировать на подобную ошибку, используйте свойство OnLifetimeError контейнера:
```C#
// Бросить исключение
container.OnLifetimeError = Reaction.ThrowException;
// Вывести ошибку в лог Юнити
container.OnLifetimeError = Reaction.LogError;
// Вывести предупреждение в лог Юнити
container.OnLifetimeError = Reaction.LogWarning;
// Проигнорировать ошибку
container.OnLifetimeError = Reaction.Ignore;
```

**7. Factory methods**

CryoDI позволяет задавать методы, которые будут вызваны контейнером для создания объекта нужного типа. Это может быть полезно, когда вам нужно создать объект каким то специфическим образом, например десериализовать из json
```C#
container.RegisterType<MyClassA>(CreateClassA);
container.RegisterSingleton<MyClassB>(CreateClassB);
    
    
...
    
private MyClassA CreateClassA()
{
   // этот метод будет вызываться каждый раз, когда контейнеру потребуется объект MyClassA
   // тут создаем объект MyClassA по своему рецепту
   ...
}
    
private MyClassB CreateClassB()
{
   // этот метод будет вызван при первом запросе объекта MyClassB
   // тут создаем объект MyClassB по своему рецепту
   ...
}
```

**8. IInitializable**

Как узнать, в какой момент в классе проставлены все зависимости, а так же зависимости этих зависимостей?
Для этого используйте интерфейс IInitializable:
```C#
using CryoDI;

public class PlayerController : IInitializable
{
	[Dependency]
	private IUserInput UserInput { get; set; }

	public void Initialize()
	{
		// тут спокойно обращаемся к нашим зависимостям - они уже проставлены и готовы к использованию
		UserInput.FirePressed += OnFirePressed;
		...
	}
	
	...
}
```

**9. Параметры**

Механизм dependency injection позволяет внедрять в класс зависимости, которые сконфигурированы в контейнере. Все возможные зависимости должны быть определены в момент конфигурирования контейнера. Что делать, если это не так?

TODO: дополнить документацию

**10. Циклические зависимости**

TODO: дополнить документацию

**11. Как использовать CryoDI вне Unity - простой пример создания контейнера и класса с зависимостями**

CryoDI можно использовать не только в Unity, но и в любом C# приложении. Например можно написать общий код, который будет выполняться и на Unity клиенте, и на игровом сервере.
```C#
using CryoDI;

public class HelloSayer
{
    public void SayHelloTo(string name)
    {
        Debug.Log(“Hello, “ + name + “!”);
    }
}

public class MyClass
{
    [Dependency]
    private HelloSayer Sayer { get; set; }

    public void SayHelloWorld()
    {
        Sayer.SayHelloTo(“World");
    }
}

var container = new CryoContainer();
container.RegisterType<HelloSayer>();
container.RegisterType<MyClass>();
var myClass = container.Resolve<MyClass>();
```
