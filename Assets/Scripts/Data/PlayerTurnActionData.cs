public partial class Player
{
    private struct TurnActionData
    {
        public enum Type
        {
            None,

            Move,
            Box,
        }

        public static TurnActionData None => new TurnActionData { myType = Type.None, myMoveDirection = Direction.Up, myMoveableBox = null };

        public bool myConsumesTurn => myType != Type.None;

        public Type myType;

        /// <summary>
        /// Used if <see cref="myType"/> is <see cref="Type.Move"/>.
        /// </summary>
        public Direction myMoveDirection;

        /// <summary>
        /// Used if <see cref="myType"/> is <see cref="Type.Box"/>.
        /// </summary>
        public MoveableBox myMoveableBox;

        public static TurnActionData CreateMove(Direction aDirection)
        {
            return new TurnActionData
            {
                myType = Type.Move,

                myMoveDirection = aDirection,

                myMoveableBox = null,
            };
        }

        public static TurnActionData CreateBox(MoveableBox aMoveableBox)
        {
            return new TurnActionData
            {
                myType = Type.Box,

                myMoveDirection = Direction.Up,

                myMoveableBox = aMoveableBox,
            };
        }
    }

}
