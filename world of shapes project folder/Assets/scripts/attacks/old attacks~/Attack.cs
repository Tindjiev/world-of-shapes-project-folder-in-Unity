using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : EntityNotCharacter
{


    //control variables
    public timelib.timer cooldown = new timelib.timer();

    //stats
    public float cooldownTimer
    {
        get
        {
            return cooldown.TotalCooldown;
        }
        set
        {
            cooldown.setCounter(value);
        }
    }
    public float Reach = 10f;

    //function variables
    //public mylib.boolfunctionKeycode inputfunction;
    //public mylib.boolfunctionbool activate;

    public abstract bool activate(bool input);

    protected abstract void initiateAttack();
    protected abstract void disableAttack();

    public abstract bool inputfunction(KeyCode key);

    protected void Awake()
    {

    }
    protected void Start()
    {

    }


    /*  public void stop()
      {
          resetCoolDown();
          actionstop();
      }*/

    //public mylib.voidfunction actionstop = mylib.donothing;

    public Texture2D defaultImage;




    public bool checkcooldown() //returns true if attack is not on cooldown and is ready
    {
        return cooldown.checkIfTimePassed();
    }
    public bool isOnCooldown() //returns true if attack is on cooldown
    {
        return cooldown.checkIfTimeNotPassed();
    }

    public void resetCoolDown()
    {
        cooldown.startTimer();
    }


    public void resetCoolDown(float tempCooldown)
    {
        cooldown.startTimerWithTempCounter(tempCooldown);
        //LastTimeUsed = Time.time + tempCooldown - cooldown;
    }


    public void reduceCdTimeRemaining(float SecondsToReduce)
    {
        cooldown.reduceTimeRemaining(SecondsToReduce);
    }


    protected abstract class AttackState : IState
    {
        protected AttackStateMachine _ASM;

        protected AttackState(AttackStateMachine ASM)
        {
            _ASM = ASM;
        }
        public abstract void OnStateEnter();

        public abstract void LogicalUpdate();

        public abstract void OnStateExit();
    }

    protected class WaitingInput : AttackState
    {

        protected Attack _attack;

        public WaitingInput(AttackStateMachine ASM, Attack attack) : base(ASM)
        {
            _attack = attack;
        }

        public override void OnStateEnter()
        {
            _attack.enabled = false;
        }

        public override void LogicalUpdate()
        {
        }

        public override void OnStateExit()
        {
            _attack.enabled = true;
        }
    }
}


public abstract class UltAttack : Attack
{



}

public interface Projectile
{
    void blocked();

    Attack getAttack();
}


public class AttackStateMachine : StateMachine
{


}




/*
interface OperandIf
{
    public void addDigit(char ch);
    public void deleteLastDigit();
    public void complete();
}
abstract class Operandac: OperandIf
{
    public abstract void addDigit(char ch);
    public abstract void deleteLastDigit();
    public void complete()
    {

    }
}
class Operandacsub : Operandac
{
    public override void addDigit(char ch)
    {

    }
    public override void deleteLastDigit()
    {

    }
}
public static class test
{
    static void main()
    {

        Operandacsub osub = new Operandacsub();

        Operandac oac = osub;

        OperandIf oif = osub;
        OperandIf oif2 = oac;

        oac.deleteLastDigit();
        osub.deleteLastDigit();
        oif.deleteLastDigit();
        oif2.deleteLastDigit();

        testmethod(oac);
        testmethod(osub);

    }

    static void testmethod(Operandac opif)
    {
        opif.deleteLastDigit();
    }
}*/