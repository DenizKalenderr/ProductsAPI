namespace ProductsAPI.DTO
{
    public class ProductDTO
    {
        //Kullanıcıya göndermek istediğimiz bilgileri yazdık. IsActive i almadık.
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
    }
}