namespace Core.LoadSystem
{
    public interface ISerializable<T>
    {
        public T SerializeSelf();
        public void DeserializeSelf(T serialized);
    }
}