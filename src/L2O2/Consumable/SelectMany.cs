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

        internal class SelectManyImpl<T, U> : Transmutation<T, (T, IEnumerable<U>)>
        {
            private readonly Func<T, IEnumerable<U>> collectionSelector;

            public SelectManyImpl(Func<T, IEnumerable<U>> collectionSelector) =>
                this.collectionSelector = collectionSelector;

            public override Chain<T, V> Compose<V>(Chain<(T, IEnumerable<U>), V> next) =>
                new Activity<V>(next, collectionSelector);

            private class Activity<V> : Activity<T, (T, IEnumerable<U>), V>
            {
                private readonly Func<T, IEnumerable<U>> collectionSelector;

                public Activity(Chain<(T, IEnumerable<U>)> next, Func<T, IEnumerable<U>> collectionSelector) : base(next) =>
                    this.collectionSelector = collectionSelector;

                public override ProcessNextResult ProcessNext(T input) =>
                    Next((input, collectionSelector(input)));
            }
        }

        internal class SelectManyIndexedImpl<T, U> : Transmutation<T, (T, IEnumerable<U>)>
        {
            private readonly Func<T, int, IEnumerable<U>> collectionSelector;

            public SelectManyIndexedImpl(Func<T, int, IEnumerable<U>> collectionSelector) =>
                this.collectionSelector = collectionSelector;

            public override Chain<T, V> Compose<V>(Chain<(T, IEnumerable<U>), V> next) =>
                new Activity<V>(next, collectionSelector);

            private class Activity<V> : Activity<T, (T, IEnumerable<U>), V>
            {
                private readonly Func<T, int, IEnumerable<U>> collectionSelector;
                private int index = 0;

                public Activity(Chain<(T, IEnumerable<U>)> next, Func<T, int, IEnumerable<U>> collectionSelector) : base(next) =>
                    this.collectionSelector = collectionSelector;

                public override ProcessNextResult ProcessNext(T input) =>
                    Next((input, collectionSelector(input, index++)));
            }
        }

        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            if (source == null) { throw new ArgumentNullException("source"); }
            if (collectionSelector == null) { throw new ArgumentNullException("collectionSelector"); }
            if (resultSelector == null) { throw new ArgumentNullException("resultSelector"); }

            var selectMany = Utils.PushTransform(source, new SelectManyImpl<TSource, TCollection>(collectionSelector));

            return new ConsumableSelectMany<TSource, TCollection, TResult, TResult, TResult>(selectMany, resultSelector, IdentityTransform<TResult>.Instance, IdentityTransform<TResult>.Instance);
        }

        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, int, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            var selectMany = Utils.PushTransform(source, new SelectManyIndexedImpl<TSource, TCollection>(collectionSelector));

            return new ConsumableSelectMany<TSource, TCollection, TResult, TResult, TResult>(selectMany, resultSelector, IdentityTransform<TResult>.Instance, IdentityTransform<TResult>.Instance);
        }

    }
}
