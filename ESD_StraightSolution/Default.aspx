<%@ Page Title="Scaled Images" Language="C#" MasterPageFile="~/Site.Master"  AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ESD_StraightSolution._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <!-- import jQuery (window_close_handler needs it) and window_close_handler for window management postback -->
    <script type="text/javascript" src="/scripts/jquery-1.10.2.min.js"></script> 
    <script src="/Scripts/window_close_handler.js"></script>

    <!-- Import page styling so things look at least a little gussied up -->
    <link rel="stylesheet" type="text/css" href="/Content/default.css" media="screen" />

    <!-- this is the value which window_close_handler will use to ensure this client window can be 
         uniquely tracked -->
    <asp:HiddenField ID="WindowID" runat="server" />

    <!-- stacking a few divs to make this page look not-too-godawful
         (styling for this is in Content\default.css -->
    <div class="fs-images-container">
        <div class="image-frame">
            <div><asp:Literal ID="OriginalLabel" runat="server" Text='Image, original size:' /></div>
            <asp:Image ID="Original" runat="server" style="margin: 5px;" />
        </div>

        <div class="image-frame">
            <div><asp:Literal ID="SixtyLabel" runat="server" Text='Image, 60% size:' /></div>
            <asp:Image ID="Sixty" runat="server" style="margin: 5px;" />
        </div>

        <div class="image-frame">
            <div><asp:Literal ID="TwentyFiveLabel" runat="server" Text='Image, 25% size:' /></div>
            <asp:Image ID="TwentyFive" runat="server" style="margin: 5px;" />
        </div>
    </div>

    <div class="db-images-container">
        <div class="image-frame">
            <div><asp:Literal ID="DBOriginalLabel" runat="server" Text='DB image, original size:' /></div>
            <asp:Image ID="DBOriginal" runat="server" style="margin: 5px;" />
        </div>

        <div class="image-frame">
            <div><asp:Literal ID="DBSixtyLabel" runat="server" Text='DB image, 60% size:' /></div>
            <asp:Image ID="DBSixty" runat="server" style="margin: 5px;" />
        </div>

        <div class="image-frame">
            <div><asp:Literal ID="DBTwentyFiveLabel" runat="server" Text='DB image, 25% size:' /></div>
            <asp:Image ID="DBTwentyFive" runat="server" style="margin: 5px;" />
        </div>
    </div>

</asp:Content>
