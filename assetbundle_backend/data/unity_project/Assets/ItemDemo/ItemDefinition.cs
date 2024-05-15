namespace ItemDemo
{
    public abstract class ItemDefinition
    {
        public abstract int visualsId { get; }
        public abstract int maxCapacity { get; }

        public abstract void Use(ref ItemContainer container);
        public abstract void Attack(ref ItemContainer container);
    }
}