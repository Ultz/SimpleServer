﻿// BadRequestException.cs - Ultz.SimpleServer.Minimal
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

using System;

#endregion

namespace Ultz.SimpleServer.Internals.Http
{
    /// <inheritdoc />
    public class BadRequestException : Exception
    {
        /// <inheritdoc />
        public BadRequestException() : base("Bad Request")
        {
        }

        /// <inheritdoc />
        public BadRequestException(Exception inner) : base("Bad Request", inner)
        {
        }

        /// <inheritdoc />
        public BadRequestException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public BadRequestException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}