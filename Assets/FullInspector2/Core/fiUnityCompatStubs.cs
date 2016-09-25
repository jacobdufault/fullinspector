#if UNITY_4_3 || UNITY_4_5
namespace UnityEngine.Serialization
{
  /// <summary>Not available - this is a stub!</summary>
  [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
  public class FormerlySerializedAsAttribute : System.Attribute
  {
    private string m_oldName;

    public string oldName
    {
      get
      {
        return this.m_oldName;
      }
    }

    public FormerlySerializedAsAttribute(string oldName)
    {
      this.m_oldName = oldName;
    }
  }
}
#endif