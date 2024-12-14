namespace ytgenerator.Shared.Dtos
{
    public class GenerateContentDto
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public List<ContentSection> Content { get; set; }
    }

    public class ContentSection
    {
        public string SectionTitle { get; set; }
        public string Content { get; set; }
    }

    public class GenerateDocxDto
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public string FilePathSeo { get; set; }
    }
}
