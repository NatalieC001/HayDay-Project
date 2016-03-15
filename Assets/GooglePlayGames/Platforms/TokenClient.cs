// <copyright file="TokenClient.cs" company="Google Inc.">
// Copyright (C) 2015 Google Inc.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
using System;

#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
namespace GooglePlayGames
{
    internal interface TokenClient
    {
        string GetEmail();

        string GetAccessToken();
        void GetIdToken(string serverClientId, Action<string> idTokenCallback);
        void SetRationale(string rationale);
    }
}
#endif