namespace Models
{
	public class PowerEntity
    {
        private long id;

        private string name;

        public PowerEntity()
        {
			Point = new Point();
        }

        private int connectionsCount;

		public Point Point { get; private set; }

		public long Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public double X
        {
            get
            {
                return Point.X;
            }

            set
            {
                Point.X = value;
            }
        }

        public double Y
        {
            get
            {
                return Point.Y;
            }

            set
            {
                Point.Y = value;
            }
        }

        public int ConnectionsCount
        {
            get
            {
                return connectionsCount;
            }

            set
            {
                connectionsCount = value;
            }
        }
    }
}
