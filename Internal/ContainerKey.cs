using System;

namespace CryoDI
{
	internal class ContainerKey
	{
		public ContainerKey(Type type) : this(type, null)
		{
		}

		public ContainerKey(Type type, string name)
		{
			Type = type;
			Name = name;
		}

		public Type Type { get; private set; }

		public string Name { get; private set; }

		protected bool Equals(ContainerKey other)
		{
			return (Type == other.Type) && string.Equals(Name, other.Name);
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// true if the specified object  is equal to the current object; otherwise, false.
		/// </returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ContainerKey) obj);
		}

		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		public override int GetHashCode()
		{
			unchecked
			{
				return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (Name != null ? Name.GetHashCode() : 0);
			}
		}

		public static bool operator ==(ContainerKey left, ContainerKey right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ContainerKey left, ContainerKey right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			if (Name == null)
				return "Type: " + Type.ToString();
			return "Type: " + Type.ToString() + ", Binding name: " + Name;
		}
	}
}
