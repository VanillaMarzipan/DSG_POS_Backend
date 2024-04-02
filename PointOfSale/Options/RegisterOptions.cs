namespace PointOfSale.Options
{
    public class RegisterOptions
    {
        public RegisterEndpoints Endpoints { get; set; }

        public class RegisterEndpoints
        {
            public string RegisterNumber { get; set; }
            public string ValidateRegister { get; set; }
        }
    }
}