using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ1_NetworkGraph.BFS
{
    internal class BFSPath
    {
        public static PozicijaPolja[,] BFSPronadji(LineEntity line, char[,] BFSlinije)
        {
            BFSlinije[(int)line.PocetakX, (int)line.PocetakY] = 'P';
            BFSlinije[(int)line.KrajX, (int)line.KrajY] = 'K';

            PozicijaPolja pocetak = new PozicijaPolja((int)line.PocetakX, (int)line.PocetakY);
            Queue<PozicijaPolja> queue = new Queue<PozicijaPolja>();
            queue.Enqueue(pocetak);

            bool[,] poseceno = new bool[601, 601]; //dimenzije matrice
            for (int i = 0; i <= 600; i++)
            {
                for (int j = 0; j <= 600; j++)
                {
                    poseceno[i, j] = false;
                }
            }
            poseceno[pocetak.PozX, pocetak.PozY] = true;

            PozicijaPolja[,] prev = new PozicijaPolja[601, 601];
            for (int i = 0; i <= 600; i++)
            {
                for (int j = 0; j <= 600; j++)
                {
                    prev[i, j] = null;
                }
            }

            while (queue.Count != 0)
            {
                PozicijaPolja polje = queue.Dequeue();

                //dosao do kraja
                if (BFSlinije[polje.PozX, polje.PozY] == 'K')
                {
                    BFSlinije[(int)line.PocetakX, (int)line.PocetakY] = ' ';
                    BFSlinije[(int)line.KrajX, (int)line.KrajY] = ' ';
                    return prev;
                }

                if (isValid(polje.PozX - 1, polje.PozY, poseceno, BFSlinije))
                {
                    queue.Enqueue(new PozicijaPolja(polje.PozX - 1, polje.PozY));
                    poseceno[polje.PozX - 1, polje.PozY] = true;
                    prev[polje.PozX - 1, polje.PozY] = polje;
                }

                if (isValid(polje.PozX + 1, polje.PozY, poseceno, BFSlinije))
                {
                    queue.Enqueue(new PozicijaPolja(polje.PozX + 1, polje.PozY));
                    poseceno[polje.PozX + 1, polje.PozY] = true;
                    prev[polje.PozX + 1, polje.PozY] = polje;
                }

                if (isValid(polje.PozX, polje.PozY - 1, poseceno, BFSlinije))
                {
                    queue.Enqueue(new PozicijaPolja(polje.PozX, polje.PozY - 1));
                    poseceno[polje.PozX, polje.PozY - 1] = true;
                    prev[polje.PozX, polje.PozY - 1] = polje;
                }

                if (isValid(polje.PozX, polje.PozY + 1, poseceno, BFSlinije))
                {
                    queue.Enqueue(new PozicijaPolja(polje.PozX, polje.PozY + 1));
                    poseceno[polje.PozX, polje.PozY + 1] = true;
                    prev[polje.PozX, polje.PozY + 1] = polje;
                }
            }
            BFSlinije[(int)line.PocetakX, (int)line.PocetakY] = ' ';
            BFSlinije[(int)line.KrajX, (int)line.KrajY] = ' ';
            return prev;
        }

        private static bool isValid(int pozX, int pozY, bool[,] poseceno, char[,] bFSlinije)
        {
            if (pozX >= 0 && pozY >= 0 && pozX < 600 && pozY < 600 && bFSlinije[pozX, pozY] != 'X' && poseceno[pozX, pozY] == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<PozicijaPolja> RekonstruisanjePutanje(PozicijaPolja pocetak, PozicijaPolja kraj, PozicijaPolja[,] prev)
        {
            List<PozicijaPolja> putanja = new List<PozicijaPolja>();
            PozicijaPolja polje;

            for (polje = kraj; polje != null; polje = prev[polje.PozX, polje.PozY])
            {
                putanja.Add(polje);
            }
            putanja.Reverse();

            if (putanja[0].PozX == pocetak.PozX && putanja[0].PozY == pocetak.PozY)
            {
                return putanja;
            }
            else
            {
                return null;
            }
        }
    }
}
