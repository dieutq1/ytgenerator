namespace ytgenerator.Shared.Dtos
{
    public class CudResponseDto
    {
        public bool IsSucceeded { get; set; } = true;
        public Guid Id { get; set; }
        public string Message { get; set; }
    }
}
