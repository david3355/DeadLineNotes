using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace DeadLineNotes
{
    class LangStruct
    {
        public LangStruct(string Language, string Image_Source, Uri Resource)
        {
            language = Language;
            img_source = Image_Source;
            resourceUri = Resource;
        }

        private string img_source;
        private string language;
        private Uri resourceUri;

        public string ImageSource
        {
            get { return img_source; }
        }

        public string Language
        {
            get { return language; }
        }

        public Uri ResourceUri
        {
            get { return resourceUri; }
        }


    }
}
