using System;
using System.Collections.Generic;

namespace StackAttack.Core
{
    public interface IUpgradeService
    {
        bool IsChoosing { get; }

        IReadOnlyList<UpgradeOption> Offer { get; }

        // Raised whenever the offer opens, is replaced by a queued one, or closes.
        event Action Changed;

        void Choose(int index);
    }
}
