public partial class Player
{
    private struct TurnActionData
    {
        public enum Type
        {
            None,

            Move,
        }

        public static TurnActionData None => new TurnActionData { myType = Type.None, myMoveDirection = Direction.Up };

        public bool myConsumesTurn => myType != Type.None;

        public Type myType;

        /// <summary>
        /// Used if <see cref="myType"/> is <see cref="Type.Move"/>.
        /// </summary>
        public Direction myMoveDirection;

        public static TurnActionData CreateMove(Direction aDirection)
        {
            return new TurnActionData
            {
                myType = Type.Move,

                myMoveDirection = aDirection,
            };
        }
    }

}
