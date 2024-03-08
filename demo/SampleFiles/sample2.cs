string text;
var fileStream = new FileStream(@"c:\file.txt", FileMode.Open, FileAccess.Read);
using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
{
    text = streamReader.ReadToEnd();
}