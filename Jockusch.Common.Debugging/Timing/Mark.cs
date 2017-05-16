using System;

namespace Jockusch.Common.Debugging
{
  public class Mark {
    public string Text {get;set;}
    public int Milliseconds {get;set;}
    public Mark (string text, int milliseconds) {
      this.Text = text;
      this.Milliseconds = milliseconds;
    }
    public override string ToString() {
      string r = this.Text + ": " + this.Milliseconds;
      if (this.CarriageReturn) {
        r += "\n";
      }
      return r;
    }
    public string ToString(int fromMilliseconds) {
      int elapsed = this.Milliseconds - fromMilliseconds;
      string r = this.Text + ": " + elapsed;
      if (this.CarriageReturn) {
        r += "\n";
      }
      return r;
    }
    public bool CarriageReturn {get;set;}
  }
}

