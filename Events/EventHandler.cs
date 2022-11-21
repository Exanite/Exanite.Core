﻿namespace Exanite.Core.Events
{
    /// <summary>
    ///     Similar to System.EventHandler, this represents a method that
    ///     will handle an event when it is raised.
    /// </summary>
    /// <typeparam name="TSender">
    ///     The type of object raising the event.
    /// </typeparam>
    /// <typeparam name="TEventArgs">
    ///     The type of the event data generated by the event.
    /// </typeparam>
    /// <param name="sender">
    ///     The source of the event.
    /// </param>
    /// <param name="e">
    ///     An object that contains the event data.
    /// </param>
    public delegate void EventHandler<in TSender, in TEventArgs>(TSender sender, TEventArgs e);
}