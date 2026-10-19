
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// RFC 7541 Appendix B'den gerçek kayıtlar.
// Bu, tam 257 sembollü tablonun küçük bir alt kümesidir.
var huffman = new Dictionary<byte, (int Code, int Length)>
{
    [0]         = (0x1ff8, 13), // NUL
    [(byte)' '] = (0x14,    6),
    [(byte)'/'] = (0x18,    6),
    [(byte)'0'] = (0x00,    5),
    [(byte)'1'] = (0x01,    5)
};

string input = "0/1";

var bits = new StringBuilder();

// Orijinal byte'ları Huffman bitlerine dönüştür.
foreach (byte symbol in Encoding.ASCII.GetBytes(input))
{
    var (code, length) = huffman[symbol];

    string binary = Convert.ToString(code, 2)
        .PadLeft(length, '0');

    Console.WriteLine(
        $"{(char)symbol}: {binary} ({length} bit)");

    bits.Append(binary);
}

Console.WriteLine($"Birleşim: {bits}");

// Byte sınırına EOS başlangıç bitleriyle tamamla.
int padding = (8 - bits.Length % 8) % 8;
bits.Append('1', padding);

byte[] compressed = Enumerable
    .Range(0, bits.Length / 8)
    .Select(i => Convert.ToByte(
        bits.ToString(i * 8, 8), 2))
    .ToArray();

// Örneğimizde uzunluk 127'den küçük olduğu için
// HPACK'in tek byte'lık uzunluk gösterimi yeterli.
byte firstByte = (byte)(0x80 | compressed.Length);

byte[] result = new byte[compressed.Length + 1];
result[0] = firstByte;
Array.Copy(compressed, 0, result, 1, compressed.Length);

Console.WriteLine($"HPACK: {Convert.ToHexString(result)}");
