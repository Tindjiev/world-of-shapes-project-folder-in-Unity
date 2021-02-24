using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BaseLib
{
    public static float[] LeftDivision(float[,] A, float[] B)
    {
        int ilen = A.GetLength(0), jlen = A.GetLength(1);
        float[,] AB = new float[ilen, jlen + 1];
        for (int i = 0; i < ilen; i++)
        {
            for (int j = 0; j < jlen; j++)
            {
                AB[i, j] = A[i, j];
            }
            AB[i, jlen] = B[i];
        }
        jlen = AB.GetLength(1);
        for (int i = 0; i < ilen; i++)
        {
            float divide = AB[i, i];
            for (int j = i; j < jlen; j++)
            {
                AB[i, j] /= divide;
            }
            for (int k = i + 1; k < ilen; k++)
            {
                float multiply = AB[k, i];
                for (int l = i; l < jlen; l++)
                {
                    AB[k, l] -= multiply * AB[i, l];
                }
            }
        }
        jlen = A.GetLength(1);
        float[] x = new float[ilen];
        for (int i = ilen - 1; i > -1; i--)
        {
            float b = AB[i, jlen];
            for (int j = jlen - 1; j > i; j--)
            {
                b -= AB[i, j] * x[j];
            }
            x[i] = b;
        }
        return x;
    }
    public static float[,] LeftDivision(float[,] A, float[,] B)
    {
        int ilen = A.GetLength(0), jlen = A.GetLength(1);
        float[,] AB = new float[ilen, jlen + B.GetLength(1)];
        for (int i = 0; i < ilen; i++)
        {
            for (int j = 0; j < jlen; j++)
            {
                AB[i, j] = A[i, j];
            }
            for (int j = 0; j < B.GetLength(1); j++)
            {
                AB[i, jlen + j] = B[i, j];
            }
        }
        return SolveInGaussianForm(AB);
    }
    public static float[,] RightDivision(float[,] B, float[,] A)
    {
        int ilen = A.GetLength(1), jlen = A.GetLength(0);
        float[,] AB = new float[ilen, jlen + B.GetLength(0)];
        for (int i = 0; i < ilen; i++)
        {
            for (int j = 0; j < jlen; j++)
            {
                AB[i, j] = A[j, i];
            }
            for (int j = 0; j < B.GetLength(0); j++)
            {
                AB[i, jlen + j] = B[j, i];
            }
        }
        return SolveInGaussianForm(AB);
    }

    public static float[,] SolveInGaussianForm(float [,] AB)
    {
        int ilen = AB.GetLength(0);
        int jlen = AB.GetLength(1);
        for (int i = 0; i < ilen; i++)
        {
            float divide = AB[i, i];
            for (int j = i; j < jlen; j++)
            {
                AB[i, j] /= divide;
            }
            for (int k = i + 1; k < ilen; k++)
            {
                float multiply = AB[k, i];
                for (int l = i; l < jlen; l++)
                {
                    AB[k, l] -= multiply * AB[i, l];
                }
            }
        }
        float[,] C = new float[ilen, jlen - ilen];
        for (int k = 0; k < jlen - ilen; k++)
        {
            for (int i = ilen - 1; i > -1; i--)
            {
                float b = AB[i, ilen + k];
                for (int j = ilen - 1; j > i; j--)
                {
                    b -= AB[i, j] * C[j, k];
                }
                C[i, k] = b;
            }
        }
        return C;
    }

    public static float[,] GetInverse(this float[,] A)
    {
        float[,] I = new float[A.GetLength(0), A.GetLength(1)];
        for (int i = 0; i < I.GetLength(0); i++)
        {
            I[i, i] = 1;
        }
        return LeftDivision(A, I);
    }

    public static bool[] LeftDivision(bool[,] A, bool[] B)
    {
        int ilen = A.GetLength(0), jlen = A.GetLength(1);
        LinkedList<bool[]> Rows = new LinkedList<bool[]>();
        int i;
        for (i = 0; i < ilen; i++)
        {
            bool[] Row = new bool[jlen + 1];
            for (int j = 0; j < jlen; j++)
            {
                Row[j] = A[i, j];
            }
            Row[jlen] = B[i];
            Rows.AddLast(Row);
        }
        jlen++;
        i = 0;
        for (LinkedListNode<bool[]> NodeRow = Rows.First; NodeRow != null;)
        {
            bool[] Row = NodeRow.Value;
            if (!Row[i])
            {
                LinkedListNode<bool[]> temp = NodeRow;
                NodeRow = NodeRow.Next;
                Rows.Remove(temp);
                Rows.AddLast(temp);
                continue;
            }
            for (LinkedListNode<bool[]> NodeRow2 = NodeRow.Next; NodeRow2 != null; NodeRow2 = NodeRow2.Next)
            {
                bool[] Row2 = NodeRow2.Value;
                if (Row2[i])
                {
                    for (int l = i; l < jlen; l++)
                    {
                        Row2[l] ^= Row[l];
                    }
                }
            }
            NodeRow = NodeRow.Next;
            i++;
        }
        jlen--;
        bool[] x = new bool[ilen];
        i--;
        for (LinkedListNode<bool[]> NodeRow = Rows.Last; NodeRow != null; NodeRow = NodeRow.Previous, i--)
        {
            bool[] Row = NodeRow.Value;
            bool b = Row[jlen];
            for (int j = jlen - 1; j > i; j--)
            {
                b ^= Row[j] && x[j];
            }
            x[i] = b;
        }
        return x;
    }



    public static bool[] SolveForAxeqbFast(bool[,] A, bool[] B)
    {
        return SolveForAxeqbFastUINF(A, B);
    }
    public static bool[] SolveForAxeqbFastUINT(bool[,] A, bool[] B)
    {
        int ilen = A.GetLength(0), jlen = A.GetLength(1);
        LinkedList<uint> Rows = new LinkedList<uint>();
        int i;
        for (i = 0; i < ilen; i++)
        {
            uint Row = 0u;
            uint temp = 1u;
            for (int j = 0; j < jlen; j++, temp <<= 1)
            {
                Row ^= A[i, j] ? temp : 0u;
            }
            Row ^= B[i] ? temp : 0u;
            Rows.AddLast(Row);
        }
        i = 0;
        uint onezeros = 1u << ilen;
        int zeroRows = 0;
        LinkedListNode<uint> NodeRow = Rows.First;
        for (uint shifted = 1u; NodeRow != null;)
        {
            if (NodeRow.Value  == onezeros)
            {
                return null;
            }
            if ((NodeRow.Value & shifted) == 0u)
            {
                LinkedListNode<uint> temp = NodeRow;
                NodeRow = NodeRow.Next;
                Rows.Remove(temp);
                Rows.AddLast(temp);
                zeroRows++;
                if (zeroRows == ilen - i)
                {
                    break;
                }
                continue;
            }
            zeroRows = 0;
            for (LinkedListNode<uint> NodeRow2 = NodeRow.Next; NodeRow2 != null; NodeRow2 = NodeRow2.Next)
            {
                if ((NodeRow2.Value & shifted) != 0u)
                {
                    NodeRow2.Value ^= NodeRow.Value;
                }
            }
            NodeRow = NodeRow.Next;
            shifted <<= 1;
            i++;
        }
        bool[] x = new bool[ilen];
        if (NodeRow == null)
        {
            NodeRow = Rows.Last;
        }
        else
        {
            NodeRow = NodeRow.Previous;
        }
        for (int jstart = --i; NodeRow != null; NodeRow = NodeRow.Previous, i--)
        {
            uint Row = NodeRow.Value;
            bool b = (Row & onezeros) != 0u;
            uint temp = 1u << jstart;
            for (int j = jstart; j > i; j--)
            {
                b ^= ((Row & temp) != 0) && x[j];
                temp >>= 1;
            }
            x[i] = b;
        }
        return x;
    }
    public static bool[] SolveForAxeqbFastULONG(bool[,] A, bool[] B)
    {
        int ilen = A.GetLength(0), jlen = A.GetLength(1);
        LinkedList<ulong> Rows = new LinkedList<ulong>();
        int i;
        for (i = 0; i < ilen; i++)
        {
            ulong Row = 0ul;
            ulong temp = 1ul;
            for (int j = 0; j < jlen; j++, temp <<= 1)
            {
                Row ^= A[i, j] ? temp : 0ul;
            }
            Row ^= B[i] ? temp : 0ul;
            Rows.AddLast(Row);
        }
        i = 0;
        ulong onezeros = 1ul << ilen;
        int zeroRows = 0;
        LinkedListNode<ulong> NodeRow = Rows.First;
        for (ulong shifted = 1ul; NodeRow != null;)
        {
            if (NodeRow.Value == onezeros)
            {
                return null;
            }
            if ((NodeRow.Value & shifted) == 0ul)
            {
                LinkedListNode<ulong> temp = NodeRow;
                NodeRow = NodeRow.Next;
                Rows.Remove(temp);
                Rows.AddLast(temp);
                zeroRows++;
                if (zeroRows == ilen - i)
                {
                    break;
                }
                continue;
            }
            zeroRows = 0;
            for (LinkedListNode<ulong> NodeRow2 = NodeRow.Next; NodeRow2 != null; NodeRow2 = NodeRow2.Next)
            {
                if ((NodeRow2.Value & shifted) != 0ul)
                {
                    NodeRow2.Value ^= NodeRow.Value;
                }
            }
            NodeRow = NodeRow.Next;
            shifted <<= 1;
            i++;
        }
        bool[] x = new bool[ilen];
        if (NodeRow == null)
        {
            NodeRow = Rows.Last;
        }
        else
        {
            NodeRow = NodeRow.Previous;
        }
        for (int jstart = --i; NodeRow != null; NodeRow = NodeRow.Previous, i--)
        {
            ulong Row = NodeRow.Value;
            bool b = (Row & onezeros) != 0ul;
            ulong temp = 1ul << jstart;
            for (int j = jstart; j > i; j--)
            {
                b ^= ((Row & temp) != 0) && x[j];
                temp >>= 1;
            }
            x[i] = b;
        }
        return x;
    }
    public static bool[] SolveForAxeqbFastUINF(bool[,] A, bool[] B)
    {
        int ilen = A.GetLength(0), jlen = A.GetLength(1) + 1;
        LinkedList<Uinf> Rows = new LinkedList<Uinf>();
        int i;
        for (i = 0; i < ilen; i++)
        {
            Uinf Row = new Uinf(jlen);
            Uinf temp = new Uinf(1ul, jlen);
            for (int j = 0; j < jlen - 1; j++, temp <<= 1)
            {
                if (A[i, j])
                {
                    Row ^= temp;
                }
            }
            if (B[i])
            {
                Row ^= temp;
            }
            Rows.AddLast(Row);
        }
        i = 0;
        Uinf onezeros = new Uinf(1ul, jlen) << ilen;
        int zeroRows = 0;
        LinkedListNode<Uinf> NodeRow = Rows.First;
        for (Uinf shifted = new Uinf(1ul, jlen); NodeRow != null;)
        {
            if (NodeRow.Value == onezeros)
            {
                return null;
            }
            if ((NodeRow.Value & shifted) == 0)
            {
                LinkedListNode<Uinf> temp = NodeRow;
                NodeRow = NodeRow.Next;
                Rows.Remove(temp);
                Rows.AddLast(temp);
                zeroRows++;
                if (zeroRows == ilen - i)
                {
                    break;
                }
                continue;
            }
            zeroRows = 0;
            for (LinkedListNode<Uinf> NodeRow2 = NodeRow.Next; NodeRow2 != null; NodeRow2 = NodeRow2.Next)
            {
                if ((NodeRow2.Value & shifted) != 0ul)
                {
                    NodeRow2.Value ^= NodeRow.Value;
                }
            }
            NodeRow = NodeRow.Next;
            shifted <<= 1;
            i++;
        }
        bool[] x = new bool[ilen];
        if (NodeRow == null)
        {
            NodeRow = Rows.Last;
        }
        else
        {
            NodeRow = NodeRow.Previous;
        }
        for (int jstart = --i; NodeRow != null; NodeRow = NodeRow.Previous, i--)
        {
            Uinf Row = NodeRow.Value;
            bool b = (Row & onezeros) != 0;
            Uinf temp = new Uinf(1ul, jlen) << jstart;
            for (int j = jstart; j > i; j--)
            {
                b ^= ((Row & temp) != 0ul) && x[j];
                temp >>= 1;
            }
            x[i] = b;
        }
        return x;
    }

    public static LinkedList<Uinf> TriangulateGauss(this LinkedList<Uinf> Rows, LinkedList<int> RowsRearrangement)
    {
        int ilen = Rows.Count;
        int i;
        int zeroRows = 0;
        LinkedListNode<Uinf> NodeRow = Rows.First;
        for (i = 0, RowsRearrangement.Clear(); i < ilen; i++)
        {
            RowsRearrangement.AddLast(i);
        }
        LinkedListNode<int> NodeRowint = RowsRearrangement.First;
        i = 0;
        for (Uinf shifted = new Uinf(1ul, ilen); NodeRow != null;)
        {
            if ((NodeRow.Value & shifted) == 0ul)
            {
                NodeRow = Rows.PushToLast(NodeRow);
                NodeRowint = RowsRearrangement.PushToLast(NodeRowint);
                zeroRows++;
                if (zeroRows == ilen - i)
                {
                    RowsRearrangement.AddFirst(i + 1);
                    return Rows;
                }
                continue;
            }
            zeroRows = 0;
            for (LinkedListNode<Uinf> NodeRow2 = NodeRow.Next; NodeRow2 != null; NodeRow2 = NodeRow2.Next)
            {
                if ((NodeRow2.Value & shifted) != 0)
                {
                    NodeRow2.Value ^= NodeRow.Value;
                    
                }
            }
            NodeRow = NodeRow.Next;
            NodeRowint = NodeRowint.Next;
            shifted <<= 1;
            i++;
        }
        RowsRearrangement.AddFirst(ilen);
        return Rows;
    }
    public static LinkedList<Uinf> RearrangeRowsFromTriangulation(this LinkedList<Uinf> Rows, LinkedList<int> RowsRearrangement, int i) //this returns the original Rows just rearranged, not triangulated
    {
        int ilen = Rows.Count;
        int zeroRows = 0;
        LinkedListNode<Uinf> NodeRow = Rows.GetNodeAt(i);
        LinkedList<Uinf> UnchangedRows = new LinkedList<Uinf>(Rows);
        LinkedListNode<Uinf> UnchangedNodeRow = UnchangedRows.GetNodeAt(i);
        RowsRearrangement.Clear();
        LinkedListNode<int> NodeRowint = null;
        for (int ii = 0; ii < ilen; ii++)
        {
            RowsRearrangement.AddLast(ii);
            if (ii == i)
            {
                NodeRowint = RowsRearrangement.Last;
            }
        }
        for (Uinf shifted = new Uinf(1ul, ilen) << i; NodeRow != null;)
        {
            if ((NodeRow.Value & shifted) == 0ul)
            {
                NodeRow = Rows.PushToLast(NodeRow);
                UnchangedNodeRow = UnchangedRows.PushToLast(UnchangedNodeRow);
                NodeRowint = RowsRearrangement.PushToLast(NodeRowint);
                zeroRows++;
                if (zeroRows == ilen - i)
                {
                    RowsRearrangement.AddFirst(i);
                    return UnchangedRows;
                }
                continue;
            }
            zeroRows = 0;
            for (LinkedListNode<Uinf> NodeRow2 = NodeRow.Next; NodeRow2 != null; NodeRow2 = NodeRow2.Next)
            {
                if ((NodeRow2.Value & shifted) != 0)
                {
                    NodeRow2.Value ^= NodeRow.Value;

                }
                //Debug.Log(NodeRow2.Value);
            }
            NodeRow = NodeRow.Next;
            UnchangedNodeRow = UnchangedNodeRow.Next;
            NodeRowint = NodeRowint.Next;
            shifted <<= 1;
            i++;
        }
        RowsRearrangement.AddFirst(ilen);
        return UnchangedRows;
    }
    public static LinkedList<Uinf> TriangulateGauss(this LinkedList<Uinf> Rows, LinkedList<int> RowsRearrangement, MyLib.MyArrayList<Uinf> RowsTransposedAndGaussInfo)
    {
        int ilen = Rows.Count;
        if (RowsRearrangement.Count < ilen)
        {
            return Rows.RearrangeRowsFromTriangulation(RowsRearrangement, 0).TriangulateGauss(RowsRearrangement, RowsTransposedAndGaussInfo);
        }
        LinkedListNode<Uinf> NodeRow = Rows.First;
        RowsTransposedAndGaussInfo.Clear(Rows.First.Value.Length);
        int i = 0;
        for (Uinf shifted = new Uinf(1ul, ilen); i < RowsRearrangement.First.Value; NodeRow = NodeRow.Next, shifted <<= 1, i++)
        {
            if ((NodeRow.Value & shifted) == 0ul)
            {
                return Rows.RearrangeRowsFromTriangulation(RowsRearrangement, i).TriangulateGauss(RowsRearrangement, RowsTransposedAndGaussInfo);
            }
            RowsTransposedAndGaussInfo.Add(new Uinf(ilen));
            //Debug.Log(NodeRow.Value);
            int j = 0;

            for (LinkedListNode<Uinf> NodeRow2 = Rows.First; j < i; NodeRow2 = NodeRow2.Next, j++)
            {
                if ((NodeRow2.Value & shifted) != 0ul)
                {
                    RowsTransposedAndGaussInfo.LastElement.SetBit(j);
                }
            }
            j++;
            for (LinkedListNode<Uinf> NodeRow2 = NodeRow.Next; NodeRow2 != null; NodeRow2 = NodeRow2.Next, j++)
            {
                if ((NodeRow2.Value & shifted) != 0ul)
                {
                    NodeRow2.Value ^= NodeRow.Value;
                    RowsTransposedAndGaussInfo.LastElement.SetBit(j);
                    //Debug.Log((i - 1).ToString() + " " + RowsTransposedAndGaussInfo.LastElement.ToString());
                }
            }
        }
        return Rows;
    }

    public static Uinf TriangulateGauss(this bool[] bools, LinkedList<int> RowsRearrangement, MyLib.MyArrayList<Uinf> RowsTransposedAndGaussInfo)
    {
        return new Uinf(bools, RowsRearrangement).TriangulateGauss(RowsTransposedAndGaussInfo);
    }




    /* static void print(float [,] A)
     {
         Debug.Log("array start");
         for (int i = 0; i < A.GetLength(0); i++)
         {
             switch (A.GetLength(1)) {
                 case 4:
                     Debug.Log(new Color(A[i, 0], A[i, 1], A[i, 2], A[i, 3]));
                     break;
                 case 3:
                     Debug.Log(new Vector3(A[i, 0], A[i, 1], A[i, 2]));
                     break;
                 case 2:
                     Debug.Log(new Vector2(A[i, 0], A[i, 1]));
                     break;
                 default:
                     Debug.Log(A[i, 0]);
                     break;
         }
         }
         Debug.Log("end");
     }*/

    public static void print(LinkedList<uint> Rows, int len)
    {
        string s = "from print uint Rows\n";
        for (LinkedListNode<uint> Node = Rows.First; Node != null; Node = Node.Next)
        {
            uint Row = Node.Value;
            s += ((Row & 1u) != 0).BoolToString();
            for (int j = 1; j < len; j++)
            {
                s += ((Row & (1u << j)) != 0).BoolToString();
            }
            s += "\n";
        }
        Debug.Log(s);
    }

    public static string ListToString(this LinkedList<Uinf> Rows)
    {
        string s = "";
        for (LinkedListNode<Uinf> Node = Rows.First; Node != null; Node = Node.Next)
        {
            s += Node.Value.ToString() + "\n";
        }
        return s;
    }
    public static string ListToString<T>(this LinkedList<T> Rows) where T : unmanaged
    {
        string s = "";
        for (LinkedListNode<T> Node = Rows.First; Node != null; Node = Node.Next) 
        {
            s += Node.Value.ToString() + "\n";
        }
        return s;
    }



}



public class Uinf
{
    private const int _ULONG_BIT_SIZE = (sizeof(ulong)) << 3; //(sizeof(ulong)) * 8
    private const int _ULONG_BIT_SIZE_MODULO = _ULONG_BIT_SIZE - 1;
    private const ulong ALL_ONES = ulong.MaxValue;
    private const ulong ALL_ZEROS = 0ul;
    private const ulong ONE_ZEROS = 1ul;

    public int Length { get; private set; }
    private ulong[] _chain;
    public ulong MaskOfLastulong
    {
        get
        {
            int modLength = Length & _ULONG_BIT_SIZE_MODULO;
            return modLength == 0 ? ALL_ONES : ~(ALL_ONES << modLength);
        }
    }

    public int ActiveBitCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < _chain.Length; count += _chain[i++].BitCount()) ;
            return count;
        }
    }
    public int InactiveBitCount => Length - ActiveBitCount;

    public Uinf(Uinf number)
    {
        _chain = new ulong[number._chain.Length];
        for (int i = _chain.Length - 1; i >= 0; --i)
        {
            _chain[i] = number._chain[i];
        }
        Length = number.Length;
    }

    public Uinf(int length)
    {
        _chain = new ulong[1 + (length - 1) / _ULONG_BIT_SIZE];
        Length = length;
    }
    public Uinf(ulong number, int length)
    {
        _chain = new ulong[1 + (length - 1) / _ULONG_BIT_SIZE];
        _chain[0] = number;
        Length = length;
    }
    private Uinf(ulong[] chain, int length)
    {
        _chain = chain;
        Length = length;
    }
    public Uinf(bool[] bits)
    {
        _chain = new ulong[1 + (bits.Length - 1) / _ULONG_BIT_SIZE];
        Length = bits.Length;
        for (int i = 0; i < Length; ++i)
        {
            if (bits[i])
            {
                SetBit(i);
            }
        }
    }
    public Uinf(bool[] bits, LinkedList<int> Rearranged)
    {
        _chain = new ulong[1 + (bits.Length - 1) / _ULONG_BIT_SIZE];
        Length = bits.Length;
        LinkedListNode<int> Node = Rearranged.First.Next;
        for (int i = 0; i < Length; i++, Node = Node.Next)
        {
            if (bits[Node.Value])
            {
                SetBit(i);
            }
        }
    }

    public void Reset()
    {
        for (int i = _chain.Length - 1; i >= 0; --i)
        {
            _chain[i] = ALL_ZEROS;
        }
    }

    public void Reset(int newLength)
    {
        if (newLength != Length) Array.Resize(ref _chain, newLength / _ULONG_BIT_SIZE);
        for (int i = _chain.Length - 1; i >= 0; --i)
        {
            _chain[i] = ALL_ZEROS;
        }
    }


    public static Uinf operator ^(Uinf num1, Uinf num2)
    {
        ulong[] chain = new ulong[num1._chain.Length];
        for (int i = chain.Length - 1; i >= 0; --i)
        {
            chain[i] = num1._chain[i] ^ num2._chain[i];
        }
        return new Uinf(chain, num1.Length);
    }
    public static Uinf operator &(Uinf num1, Uinf num2)
    {
        ulong[] chain = new ulong[num1._chain.Length];
        for (int i = 0; i < chain.Length; i++)
        {
            chain[i] = num1._chain[i] & num2._chain[i];
        }
        return new Uinf(chain, num1.Length);
    }
    public static Uinf operator |(Uinf num1, Uinf num2)
    {
        ulong[] chain = new ulong[num1._chain.Length];
        for (int i = 0; i < chain.Length; i++)
        {
            chain[i] = num1._chain[i] | num2._chain[i];
        }
        return new Uinf(chain, num1.Length);
    }

    public static Uinf operator ^(Uinf num1, ulong num2)
    {
        ulong[] chain = new ulong[num1._chain.Length];
        chain[0] = num1._chain[0] ^ num2;
        for (int i = 1; i < chain.Length; i++)
        {
            chain[i] = num1._chain[i];
        }
        return new Uinf(chain, num1.Length);
    }
    public static Uinf operator &(Uinf num1, ulong num2)
    {
        ulong[] chain = new ulong[num1._chain.Length];
        chain[0] = num1._chain[0] & num2;
        return new Uinf(chain, num1.Length);
    }
    public static Uinf operator |(Uinf num1, ulong num2)
    {
        ulong[] chain = new ulong[num1._chain.Length];
        chain[0] = num1._chain[0] | num2;
        for (int i = 1; i < chain.Length; i++)
        {
            chain[i] = num1._chain[i];
        }
        return new Uinf(chain, num1.Length);
    }

    public static Uinf operator >>(Uinf num, int times)
    {
        Uinf newnum = new Uinf(num);
        if (times == 0)
        {
            return newnum;
        }
        ulong[] chain = newnum._chain;
        if (times < _ULONG_BIT_SIZE)
        {
            chain[0] >>= times;
            for (int i = 1; i < chain.Length; ++i)
            {
                chain[i - 1] |= chain[i] << (_ULONG_BIT_SIZE - times);
                chain[i] >>= times;
            }
            return newnum;
        }
        else
        {
            chain[0] >>= times; // shift operator autmatically does modulo operation with times 
            int i = 1;
            for (; i < chain.Length; ++i)
            {
                chain[i - 1] |= chain[i] << (_ULONG_BIT_SIZE - (times & _ULONG_BIT_SIZE_MODULO));
                chain[i] >>= times;
            }
            i = 0;
            for (int arrayshift = times / _ULONG_BIT_SIZE; i < chain.Length - arrayshift; ++i)
            {
                chain[i] = chain[i + arrayshift];
            }
            for (; i < chain.Length; ++i)
            {
                chain[i] = ALL_ZEROS;
            }
            return newnum;
        }
    }
    public static Uinf operator <<(Uinf num, int times)
    {
        Uinf newnum = new Uinf(num);
        if (times == 0)
        {
            return newnum;
        }
        ulong[] chain = newnum._chain;
        if (times < _ULONG_BIT_SIZE)
        {
            chain[chain.Length - 1] = (chain[chain.Length - 1] << times) & newnum.MaskOfLastulong;
            for (int i = chain.Length - 2; i >= 0; --i)
            {
                chain[i + 1] |= chain[i] >> (_ULONG_BIT_SIZE - times);
                chain[i] <<= times;
            }
            return newnum;
        }
        else
        {       // shift operator autmatically does modulo operation with times 
            chain[chain.Length - 1] = (chain[chain.Length - 1] << times) & newnum.MaskOfLastulong;
            int i = chain.Length - 2;
            for (; i >= 0; --i)
            {
                chain[i + 1] |= chain[i] >> (_ULONG_BIT_SIZE - (times & _ULONG_BIT_SIZE_MODULO));
                chain[i] <<= times;
            }
            i = chain.Length - 1;
            for (int arrayshift = times / _ULONG_BIT_SIZE; i >= arrayshift; --i)
            {
                chain[i] = chain[i - arrayshift];
            }
            for (; i >= 0; --i)
            {
                chain[i] = ALL_ZEROS;
            }
            return newnum;
        }
    }

    public bool this[int index]
    {
        get => GetBit(index);
        set
        {
            if (value) SetBit(index);
            else ResetBit(index);
        }
    }

    public Uinf InverseBit(int index)
    {
        _chain[index / _ULONG_BIT_SIZE] ^= ONE_ZEROS << index;
        return this;
    }
    public Uinf SetBit(int index)
    {
        _chain[index / _ULONG_BIT_SIZE] |= ONE_ZEROS << index;
        return this;
    }
    public Uinf ResetBit(int index)
    {
        _chain[index / _ULONG_BIT_SIZE] &= ~(ONE_ZEROS << index);
        return this;
    }
    public Uinf SetAllBits()
    {
        for (int i = _chain.Length - 2; i >= 0; --i)
        {
            _chain[i] = ALL_ONES;
        }
        _chain[_chain.Length - 1] = MaskOfLastulong;
        return this;
    }
    public Uinf ResetAllBits()
    {
        for (int i = 0; i < _chain.Length; ++i)
        {
            _chain[i] = ALL_ZEROS;
        }
        return this;
    }
    public void ResetBitsOverLength()
    {
        _chain[_chain.Length - 1] &= MaskOfLastulong;
    }

    public bool GetBit(int index)
    {
        return (_chain[index / _ULONG_BIT_SIZE] & (ONE_ZEROS << index)) != ALL_ZEROS;
    }

    public static bool operator ==(Uinf num1, Uinf num2)
    {
        for (int i = 0; i < num1._chain.Length; ++i)
        {
            if (num1._chain[i] != num2._chain[i]) return false;
        }
        return true;
    }
    public static bool operator !=(Uinf num1, Uinf num2) => !(num1 == num2);

    public static bool operator ==(Uinf num1, ulong num2)
    {
        if (num1._chain[0] != num2) return false;
        for (int i = num1._chain.Length - 1; i > 0; --i) // don't check the least signifcant array
        {
            if (num1._chain[i] != ALL_ZEROS) return false;
        }
        return true;
    }
    public static bool operator !=(Uinf num1, ulong num2) => !(num1 == num2);

    public bool IsNull
    {
        get => _chain == null;
        set
        {
            if (!value && IsNull)
            {
                _chain = new ulong[0];
                Length = 0;
            }
            else if (value && !IsNull)
            {
                _chain = null;
                Length = 0;
            }
        }
    }

    public bool Equals(Uinf other) => this == other;

    public override bool Equals(object obj) => !(obj is null) && obj is Uinf bits && Equals(bits);

    public override int GetHashCode() => 0;

    public override string ToString()
    {
        if (_chain == null) return "null";
        string s = "";
        for (int i = 0; i < Length; i++)
        {
            s += ((_chain[i / _ULONG_BIT_SIZE] & (ONE_ZEROS << i)) != 0).BoolToString();
        }
        //char[] temp = s.ToCharArray();
        //System.Array.Reverse(temp);
        //return new string(temp);
        return s;
    }


    public Uinf TriangulateGauss(MyLib.MyArrayList<Uinf> RowsTransposedAndGaussInfo)
    {
        Uinf b = new Uinf(this);
        int i = 0;
        if (!RowsTransposedAndGaussInfo.HasFreeSlots)
        {
            for (Uinf MASK = new Uinf(b.Length).SetAllBits() << 1; i < RowsTransposedAndGaussInfo.Count - 1; i++, MASK <<= 1)
            {
                if (b.GetBit(i))
                {
                    b ^= RowsTransposedAndGaussInfo[i] & MASK;
                }
            }
        }
        else
        {
            for (Uinf MASK = new Uinf(b.Length).SetAllBits() << 1; i < RowsTransposedAndGaussInfo.Count; i++, MASK <<= 1)
            {
                if (b.GetBit(i))
                {
                    b ^= RowsTransposedAndGaussInfo[i] & MASK;
                }
            }
            for (; i < b.Length; i++)
            {
                if (b.GetBit(i))
                {
                    b.IsNull = true;
                    return b;
                }
            }
        }
        return b;
    }

    public Uinf SolveForAxEQb(MyLib.MyArrayList<Uinf> RowsTransposedAndGaussInfo)
    {
        Uinf b = new Uinf(this);
        Uinf MASK = (new Uinf(b.Length).SetAllBits()) >> (1 + RowsTransposedAndGaussInfo.FreeSlots);
        Uinf shifted = new Uinf(1ul, b.Length) << (RowsTransposedAndGaussInfo.Count - 1);
        Uinf x = new Uinf(b);
        for (int i = RowsTransposedAndGaussInfo.Count - 1; i > 0; i--, MASK >>= 1, shifted >>= 1)
        {
            if ((x & shifted) != 0ul)
            {
                x ^= RowsTransposedAndGaussInfo[i] & MASK;
            }
        }
        return x;
    }
}