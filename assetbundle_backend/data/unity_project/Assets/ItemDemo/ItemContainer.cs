namespace ItemDemo
{
    public struct ItemContainer
    {
        public ItemDefinition definition;
        
        public void Use()
        {
            definition.Use(ref this);
        }
        
        public void Attack()
        {
            definition.Attack(ref this);
        }

        public bool Equals(ItemContainer other)
        {
            return Equals(definition, other.definition);
        }

        public override int GetHashCode()
        {
            return (definition != null ? definition.GetHashCode() : 0);
        }
    }
}