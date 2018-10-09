namespace CryoDI
{
	public class Param
	{
		public Param()
		{
		}

		public Param(object value)
		{
			Value = value;
		}
		
		public Param(string name, object value)
		{
			Name = name;
			Value = value;
		}

		public string Name { get; set; }
		
		public object Value { get; set; }
	}
}