using System;
using System.Collections.Generic;

namespace MicroLib
{
    public class Store : IStore
    {
        private readonly Dictionary<string, float> internalSore;

        public Store()
        {
            this.internalSore = new Dictionary<string, float>();
        }
        public void Add(string key, float value)
        {
            this.internalSore.Add(key, value);
        }

        public float Get(string key)
        {
            if (!this.internalSore.ContainsKey(key))
            {
                throw new InvalidOperationException($"No record for key {key}.");
            }

            return this.internalSore[key];
        }

        public bool Exists(string key)
        {
            return this.internalSore.ContainsKey(key);
        }

        public IDictionary<string, float> GetAll()
        {
            return this.internalSore;
        }

        public void Update(string key, float value){
            if (this.Exists(key)){
                this.internalSore[key] = value;
            }
        }
    }
}
