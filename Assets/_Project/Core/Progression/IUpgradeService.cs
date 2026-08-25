using System.Collections.Generic;

namespace StackAttack.Core
{
    public interface IUpgradeService
    {
        bool IsChoosing { get; }

        // Bumped on every roll so a screen can tell a fresh offer from the one it is
        // already showing without comparing the option list itself.
        int OfferId { get; }

        IReadOnlyList<UpgradeOption> Offer { get; }

        void Choose(int index);
    }
}
