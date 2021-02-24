using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsOutSolver : MonoBehaviour
{
    private LightsOutScript _lightsOutVars;
    private Uinf _solution, _estimatedState;
    private const float _SECONDS_FOR_NEXT = 0.15f;
    private readonly Timer _waitForNext = new Timer(_SECONDS_FOR_NEXT);
    public SolveType SolveStyle = SolveType.SerialAndOpposites;
    private System.Action _doSolutionStep
    {
        get
        {
            switch (SolveStyle)
            {
                case SolveType.Serial:
                    return DoSolutionStepSerial;
                case SolveType.SerialAndOpposites:
                    return DoSolutionStepSerialAndOpposite;
                default:
                    return null;
            }
        }
    }

    private bool _solving = false;

    public enum SolveType
    {
        Serial,
        SerialAndOpposites,
    }

    private void Awake()
    {
        _lightsOutVars = this.SearchComponent<LightsOutScript>();
    }

    private void Update()
    {
        if (!_solving && _lightsOutVars.B != 0ul)
        {
            StartSolving();
        }
        if (_solving && _waitForNext.CheckIfTimePassed)
        {
            if (_estimatedState != _lightsOutVars.B)
            {
                StartSolving();
            }
            _doSolutionStep();
        }
    }

    private void StartSolving()
    {
        _solution = _lightsOutVars.GetSolution();
        _estimatedState = new Uinf(_lightsOutVars.B);
        _solving = true;
        _solverSquareIndex = 0;
        _waitForNext.StartTimer();
    }

    private void DoSolutionStepSerial()
    {
        FindNextSquare();
        if (_solverSquareIndex != -1)
        {
            _lightsOutVars.ChangeSquareAndNeighbors(_lightsOutVars.BoardArray[_solverSquareIndex / _lightsOutVars.BoardArray.GetLength(0), _solverSquareIndex % _lightsOutVars.BoardArray.GetLength(1)]);
            _estimatedState.ResetBit(_solverSquareIndex++);
            _waitForNext.StartTimer();
        }
    }
    private void DoSolutionStepSerialAndOpposite()
    {
        FindNextSquare();
        FindNextSquareFromEnd();
        if (_solverSquareIndex != -1)
        {
            _lightsOutVars.ChangeSquareAndNeighbors(_lightsOutVars.BoardArray[_solverSquareIndex / _lightsOutVars.BoardArray.GetLength(0), _solverSquareIndex % _lightsOutVars.BoardArray.GetLength(1)]);
            _estimatedState.ResetBit(_solverSquareIndex++);
            if (_solverSquareIndex2 != -1)
            {
                _lightsOutVars.ChangeSquareAndNeighbors(_lightsOutVars.BoardArray[_solverSquareIndex2 / _lightsOutVars.BoardArray.GetLength(0), _solverSquareIndex2 % _lightsOutVars.BoardArray.GetLength(1)]);
                _estimatedState.ResetBit(_solverSquareIndex2--);
            }
            _waitForNext.StartTimer();
        }
    }

    private int _solverSquareIndex;
    private void FindNextSquare()
    {
        for (int i = _solverSquareIndex; i < _solution.Length; i++)
        {
            if (_solution[i])
            {
                _solverSquareIndex = i;
                _solution.ResetBit(i);
                return;
            }
        }
        _solving = false;
        _solverSquareIndex = -1;
    }

    private int _solverSquareIndex2;
    private void FindNextSquareFromEnd()
    {
        for (int i = _solution.Length - 1; i > _solverSquareIndex2; i--)
        {
            if (_solution[i])
            {
                if (i == _solverSquareIndex) break;
                _solverSquareIndex2 = i;
                return;
            }
        }
        _solverSquareIndex2 = -1;
    }
}
