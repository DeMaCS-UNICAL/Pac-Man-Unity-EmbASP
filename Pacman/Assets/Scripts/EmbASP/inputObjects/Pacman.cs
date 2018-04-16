namespace Assets.Scripts.EmbASP {
	using Id = it.unical.mat.embasp.languages.Id;
	using Param = it.unical.mat.embasp.languages.Param;

[Id("pacman")]
	public class Pacman
	{
    [Param(0)]
		private int x;
    [Param(1)]
    private int y;

		public Pacman() { }

		public Pacman(int x, int y) : base()
		{
			this.x = x;
			this.y = y;
		}

    public virtual int getX() {
      return x;
    }

    public virtual void setX(int value) {
      this.x = value;
    }

    public virtual int getY() {
      return y;
    }

    public virtual void setY(int value) {
      this.y = value;
    }
  }
}