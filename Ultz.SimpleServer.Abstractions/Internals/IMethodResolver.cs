﻿// IMethodResolver.cs - Ultz.SimpleServer.Abstractions
// 
// Copyright (C) 2018 Ultz Limited
// 
// This file is part of SimpleServer.
// 
// SimpleServer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// SimpleServer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with SimpleServer. If not, see <http://www.gnu.org/licenses/>.

#region

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Ultz.SimpleServer.Internals
{
    /// <summary>
    ///     Uses a <see cref="Dictionary{TKey,TValue}" /> to resolve <see cref="IMethod" />s by their id
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MethodResolver<T> : IMethodResolver where T : IMethod
    {
        private readonly Dictionary<byte[], T> _methods;

        /// <summary>
        ///     Creates an instance of this method resolver, with the given dictionary of methods
        /// </summary>
        /// <param name="dictionary">the dictionary of methods, indexed by their IDs</param>
        public MethodResolver(Dictionary<byte[], T> dictionary)
        {
            _methods = dictionary;
        }

        IMethod IMethodResolver.GetMethod(byte[] id)
        {
            return GetMethod(id);
        }

        /// <summary>
        ///     Resolves an <see cref="IMethod" /> with the given ID. Will return null on failure.
        /// </summary>
        /// <param name="id">the method ID to search for</param>
        /// <returns>the resolved <see cref="IMethod" />, or null if a method was not found.</returns>
        public T GetMethod(byte[] id)
        {
            return _methods.First(x => x.Key.SequenceEqual(id)).Value;
        }
    }

    /// <summary>
    ///     Provides the structure for a class that can resolve methods by their IDs.
    /// </summary>
    public interface IMethodResolver
    {
        /// <summary>
        ///     Resolves an <see cref="IMethod" /> with the given ID. Will return null on failure.
        /// </summary>
        /// <param name="id">the method ID to search for</param>
        /// <returns>the resolved <see cref="IMethod" />, or null if a method was not found.</returns>
        IMethod GetMethod(byte[] id);
    }
}