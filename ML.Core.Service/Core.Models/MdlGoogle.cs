using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class MdlGoogle
    {
        public class Feature2
        {
            public string type { get; set; }
        }

        public class Image2
        {
            public string content { get; set; }
        }

        public class Request2
        {
            public Image2 image { get; set; }
            public List<Feature2> features { get; set; }
        }

        public class Root2
        {
            public List<Request2> requests { get; set; }
        }

        public class Block
        {
            public BoundingBox boundingBox { get; set; }
            public List<Paragraph> paragraphs { get; set; }
            public string blockType { get; set; }
        }

        public class BoundingBox
        {
            public List<Vertex> vertices { get; set; }
        }

        public class BoundingPoly
        {
            public List<Vertex> vertices { get; set; }
        }

        public class DetectedBreak
        {
            public string type { get; set; }
        }

        public class FullTextAnnotation
        {
            public List<Page> pages { get; set; }
            public string text { get; set; }
        }

        public class Page
        {
            public int width { get; set; }
            public int height { get; set; }
            public List<Block> blocks { get; set; }
        }

        public class Paragraph
        {
            public BoundingBox boundingBox { get; set; }
            public List<Word> words { get; set; }
        }

        public class Property
        {
            public DetectedBreak detectedBreak { get; set; }
        }

        public class Response
        {
            public List<TextAnnotation> textAnnotations { get; set; }
            public FullTextAnnotation fullTextAnnotation { get; set; }
        }

        public class RootRes
        {
            public List<Response> responses { get; set; }
        }

        public class Symbol
        {
            public BoundingBox boundingBox { get; set; }
            public string text { get; set; }
            public Property property { get; set; }
        }

        public class TextAnnotation
        {
            public string locale { get; set; }
            public string description { get; set; }
            public BoundingPoly boundingPoly { get; set; }
        }

        public class Vertex
        {
            public int x { get; set; }
            public int y { get; set; }
        }

        public class Word
        {
            public BoundingBox boundingBox { get; set; }
            public List<Symbol> symbols { get; set; }
        }



    }


    public class MdlGenPage
    {
        public  int MaxWidth { get; set; }
        public int MaxHeight { get; set; }

        public List<MdlGRes> mdlGRes { get; set; }
    }

    public class MdlGRes
    {
        public string ocrstr { get; set; }
        public string AType { get; set; }    
        public Rectangle rectangle { get; set; }
    }
}
