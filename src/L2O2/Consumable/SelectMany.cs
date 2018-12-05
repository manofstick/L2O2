#region Copyright and license information
// Copyright 2010-2011 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using L2O2.Core;
using System;
using System.Collections.Generic;

namespace L2O2
{
    public static partial class Consumable
    {
        public static IEnumerable<TResult> SelectMany<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TResult>> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            return new ConsumableSelectMany<TResult, TResult, TResult>(Consumable.Select(source, selector), IdentityTransform<TResult>.Instance, IdentityTransform<TResult>.Instance);
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, int, IEnumerable<TResult>> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            return new ConsumableSelectMany<TResult, TResult, TResult>(Consumable.Select(source, selector), IdentityTransform<TResult>.Instance, IdentityTransform<TResult>.Instance);
        }

        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (collectionSelector == null)
            {
                throw new ArgumentNullException("collectionSelector");
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }
            return SelectManyImpl(source, collectionSelector, resultSelector);
        }

        private static IEnumerable<TResult> SelectManyImpl<TSource, TCollection, TResult>(
            IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            foreach (TSource item in source)
            {
                foreach (TCollection collectionItem in collectionSelector(item))
                {
                    yield return resultSelector(item, collectionItem);
                }
            }
        }

        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, int, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (collectionSelector == null)
            {
                throw new ArgumentNullException("collectionSelector");
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }
            return SelectManyImpl(source, collectionSelector, resultSelector);
        }

        private static IEnumerable<TResult> SelectManyImpl<TSource, TCollection, TResult>(
            IEnumerable<TSource> source,
            Func<TSource, int, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            int index = 0;
            foreach (TSource item in source)
            {
                foreach (TCollection collectionItem in collectionSelector(item, index++))
                {
                    yield return resultSelector(item, collectionItem);
                }
            }
        }
    }
}
