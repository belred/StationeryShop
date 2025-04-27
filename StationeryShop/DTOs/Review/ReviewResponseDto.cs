namespace StationeryShop.DTOs.Review
{
    public class ReviewResponseDto
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public ReviewerDto Author { get; set; }

        public class ReviewerDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}