// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SpreadsheetUtilities
{
    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two 
    ///ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 
    ///equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an 
    ///element to a
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is
    ///called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is
    ///called dependees(s).
    ///        (The set of things that s depends on) 
    ///
    /// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    ///     dependents("a") = {"b", "c"}
    ///     dependents("b") = {"d"}
    ///     dependents("c") = {}
    ///     dependents("d") = {"d"}
    ///     dependees("a") = {}
    ///     dependees("b") = {"a"}
    ///     dependees("c") = {"a"}
    ///     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        private Dictionary<string, HashSet<string>> Dependents;
        private Dictionary<string, HashSet<string>> Dependees;
        private int dependencyCount;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        /// 
        public DependencyGraph()
        {
            Dependees = new Dictionary<string, HashSet<string>>();
            Dependents = new Dictionary<string, HashSet<string>>();
            dependencyCount = 0;
        }

        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return dependencyCount; }
        }

        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you 
        /// would invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        { 
            get 
            {
                if (!Dependents.ContainsKey(s))
                    return 0;
                return Dependees[s].Count; 
            }
        }


        /// <summary>
        /// Reports whether the dependency at string s is not empty
        /// </summary>
        /// <param name="s">the dependent to cehck</param>
        /// <returns>true if there is a dependency with the given string, false otherwise</returns>
        public bool HasDependents(string s)
        {
            if(Dependents.ContainsKey(s))
                return Dependents[s].Count > 0;
            return false;
        }

        /// <summary>
        /// Reports whether the dependee at string s is not empty
        /// </summary>
        /// <param name="s">The dependee to be checked</param>
        /// <returns> true if s has dependees, false otherwise </returns>
        public bool HasDependees(string s)
        {
            if(Dependees.ContainsKey(s))
                return Dependees[s].Count > 0;
            return false;
        }

        /// <summary>
        /// Enumerates Dependents given a string
        /// </summary>
        /// <param name="s">string key to enumerate dependents</param>
        /// <returns>The IEnumerable object which contains the dependents</returns>
        public IEnumerable<string> GetDependents(string s)
        {
            if (!Dependents.ContainsKey(s))
                return new List<string>();
            return Dependents[s];
        }

        /// <summary>
        /// Enumerates Dependees given a string
        /// </summary>
        /// <param name="s">string key to enumerate dependees</param>
        /// <returns>The IEnumerable object which contains the dependees</returns>
        public IEnumerable<string> GetDependees(string s)
        {
            if(!Dependees.ContainsKey(s))
                return new List<string>();
            return Dependees[s];
        }

        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            //these four branches are for the HasPair and ease of potential future dependencies
            //being added to the dictionaries.
            if (!Dependents.ContainsKey(s))
                Dependents.Add(s, new HashSet<string>());

            if (!Dependents.ContainsKey(t))
                Dependents.Add(t, new HashSet<string>());

            if (!Dependees.ContainsKey(s))
                Dependees.Add(s, new HashSet<string>());

            if (!Dependees.ContainsKey(t))
                Dependees.Add(t, new HashSet<string>());

            if (!HasPair(s,t))
            {
                Dependents[s].Add(t);
                Dependees[t].Add(s);
                dependencyCount++;
            }
        }

        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s">the dependent of the pair (s must be evaluated before t)</param>
        /// <param name="t">the dependee of the pair (t cannot be evaluated until s is)</param>
        public void RemoveDependency(string s, string t)
        {
            if (HasPair(s, t))
            {
                Dependees[t].Remove(s);
                Dependents[s].Remove(t);
                dependencyCount--;
            }

        }

        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        ///<param name="s">the dependent key to have all dependents removed</param>
        /// <param name="newDependents">an IEnumerable object that contains the new dependents</param>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if(Dependents.ContainsKey(s))
                foreach (string r in Dependents[s])
                    RemoveDependency(s, r);

            foreach (string t in newDependents)
                AddDependency(s, t);
        }

        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependees, adds the ordered pair (s,t).
        /// </summary>
        ///<param name="s">the dependent key to have all dependents removed</param>
        /// <param name="newDependees">an IEnumerable object that contains the new dependees</param>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            if(Dependees.ContainsKey(s))
                foreach (string r in Dependees[s])
                    RemoveDependency(r, s);

            foreach (string t in newDependees)
                AddDependency(t, s);
        }

        /// <summary>
        /// A method that checks if the graph contains the pair already.
        /// t depends on s.
        /// </summary>
        /// <param name="s">a string dependent</param>
        /// <param name="t"> a string dependee</param>
        /// <returns>true if the pair exists, false otherwise</returns>
        private bool HasPair(string s, string t)
        {
            if(!Dependents.ContainsKey(s))
                return false;

            return Dependents[s].Contains(t);
        }
    }
}