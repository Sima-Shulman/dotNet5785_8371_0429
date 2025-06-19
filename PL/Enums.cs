using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL;

internal class CallTypeCollection : IEnumerable
{
    static readonly IEnumerable<BO.Enums.CallType> ct_enums =
(Enum.GetValues(typeof(BO.Enums.CallType)) as IEnumerable<BO.Enums.CallType>)!;

    public IEnumerator GetEnumerator() => ct_enums.GetEnumerator();
}


internal class CallStatusCollection : IEnumerable
{
    static readonly IEnumerable<BO.Enums.CallStatus> ct_enums =
(Enum.GetValues(typeof(BO.Enums.CallStatus)) as IEnumerable<BO.Enums.CallStatus>)!;

    public IEnumerator GetEnumerator() => ct_enums.GetEnumerator();
}
internal class DistanceTypeCollection : IEnumerable
{
    static readonly IEnumerable<BO.Enums.DistanceType> dt_enums =
(Enum.GetValues(typeof(BO.Enums.DistanceType)) as IEnumerable<BO.Enums.DistanceType>)!;

    public IEnumerator GetEnumerator() => dt_enums.GetEnumerator();
}

