<%@ Page Title="Keepalive" Language="C#" AutoEventWireup="true" CodeBehind="Keepalive.aspx.cs" Inherits="ESD_StraightSolution._Keepalive" %>


    <!-- This would be useful if our client checked for success of the keepalive refresh, but it currently doesn't --> 
    <% Response.Write(refresh_succeeded.ToString().ToLower()); %>