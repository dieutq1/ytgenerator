using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ytgenerator.Shared.Dtos
{
    // DTOs for response mapping
    public class ApiOutLineResponse
    {
        public string Message { get; set; }
        public string Title { get; set; }
        public Outline Outline { get; set; }
    }

    public class Outline
    {
        public List<Section> Sections { get; set; }
    }

    public class Section
    {
        public string Title { get; set; }
        public List<Subsection> Subsections { get; set; }
    }

    public class Subsection
    {
        public string Subtitle { get; set; }

        public string TimeRange { get; set; }
    }

    public class VideoOutline
    {
        public string VideoUrl { get; set; }
        public string Title { get; set; }
        public List<Section> Sections { get; set; }
        public bool IsSelected { get; set; }
    }
}
