<?xml version="1.0" encoding="UTF-8"?>

<%@ Page Language="C#" AutoEventWireup="true" Inherits="CompositeC1Contrib.Email.Web.UI.StatisticsPage" %>

<html xmlns="http://www.w3.org/1999/xhtml" xmlns:ui="http://www.w3.org/1999/xhtml" xmlns:control="http://www.composite.net/ns/uicontrol">
    <control:httpheaders runat="server" />

    <head runat="server">
        <title>Statistics view</title>
        
        <control:styleloader runat="server" />
        <control:scriptloader type="sub" runat="server" />
        
        <script type="text/javascript">
            DocumentManager.isDocumentSelectable = true;
		</script>
        
        <style>
            #scrollbox > table {
                margin: 1em;
                border: none;
            }

             a {
                 text-decoration: underline;
                 cursor: pointer;
             }

            a:hover {
                text-decoration: none;
            }
        </style>
    </head>

    <body>
        <form runat="server">
            <ui:page id="statisticsView">
                <ui:scrollbox id="scrollbox">
                    First Sent: <asp:Literal ID="litFirstSent" runat="server" /> <br />
                    Last Sent: <asp:Literal ID="litLastSent" runat="server" /> <br />
                    Sent: <asp:Literal ID="litSent" runat="server" /> <br />
                    Opened: <asp:Literal ID="litOpened" runat="server" /> <br />
                    Clicked: <asp:Literal ID="litCLicked" runat="server" /> <br />
                </ui:scrollbox>
            </ui:page>
        </form>
    </body>
</html>
