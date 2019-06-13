using SBattle.UI;
using System.Windows.Media;//библиотека рисования

namespace SBattle
{

    #region Класс значений ячеек игрового поля

    static class CellValues
    {
        public static readonly BattleFieldColorsCollection BattleFieldColors = new BattleFieldColorsCollection(new[]{
            Brushes.SkyBlue,
            Brushes.Green,
            Brushes.DarkRed,
            Brushes.Crimson,
            Brushes.Ivory
        });

        public const int None = 0;
        public const int Own = 1;
        public const int Dead = 2;
        public const int CompletlyDead = 3;
        public const int FiredEmpty = 4;
    }
    #endregion

    #region Класс значений границ игрового поля и сетки

    static class BorderValues
    {
        public static readonly BattleFieldColorsCollection BattleFieldBorderColors = new BattleFieldColorsCollection(new[]{
            Brushes.Transparent,
            Brushes.Green,
            Brushes.Salmon
        });

        public const int None = 0;
        public const int PlaceSelection = 1;
        public const int TargetSelection = 2;
    }
    #endregion
}
