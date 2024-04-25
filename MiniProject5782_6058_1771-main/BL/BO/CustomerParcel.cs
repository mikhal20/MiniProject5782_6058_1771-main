namespace BO
{
    /// <summary>
    /// a class of costumerParcels: every "customerParcel has all these fields
    /// </summary>
    public class CustomerParcel //לקוח בחבילה
    {
        public int Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// allow to print a variable of customerParcel Type
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = "";
            result += $"Id: {Id}\n";
            result += $"Name: {Name}\n";
            return result;
        }
    }
}