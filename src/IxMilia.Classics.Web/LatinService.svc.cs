// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace IxMilia.Classics.Web
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class LatinService
    {
        static LatinDictionary LatinDictionary = new LatinDictionary();

        // Add more operations here and mark them with [OperationContract]
        [OperationContract]
        [WebGet]
        public object Translate(string latin)
        {
            var def = LatinDictionary.GetDefinitions(latin);
            if (def.Count() == 1 && def.Single().Parts.Count() == 1)
            {
                var entry = def.Single().Parts.Single().Stem.Entry;
                return $"{entry.Entry}: {entry.Definition}";
            }

            return "heard " + latin;
        }
    }
}
