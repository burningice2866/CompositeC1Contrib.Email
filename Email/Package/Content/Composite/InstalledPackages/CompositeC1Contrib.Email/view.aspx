<?xml version="1.0" encoding="UTF-8"?>

<%@ Page Language="C#" AutoEventWireup="true" Inherits="CompositeC1Contrib.Email.Web.UI.ViewPage" %>
<%@ Register TagPrefix="aspui" Namespace="Composite.Core.WebClient.UiControlLib" Assembly="Composite" %>

<html xmlns="http://www.w3.org/1999/xhtml" xmlns:ui="http://www.w3.org/1999/xhtml" xmlns:control="http://www.composite.net/ns/uicontrol">
    <control:httpheaders runat="server" />

    <head runat="server">
        <title>Mail view</title>
        
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
            <ui:page id="mailView">
                <ui:toolbar id="toolbar">
			        <ui:toolbarbody>
				        <ui:toolbargroup>
					        <aspui:ToolbarButton AutoPostBack="true" Text="Back" ImageUrl="${icon:back}" runat="server" OnClick="OnBack" />
                            <aspui:ToolbarButton AutoPostBack="true" Text="Delete" ImageUrl="${icon:delete}" runat="server" OnClick="OnDelete" />
                            <aspui:ToolbarButton AutoPostBack="true" Text="View log" ImageUrl="${icon:log}" runat="server" OnClick="OnLog" />
                            <aspui:ToolbarButton AutoPostBack="true" Text="Download Eml" ImageUrl="${icon:down}" runat="server" OnClick="OnDownload" />
				        </ui:toolbargroup>
			        </ui:toolbarbody>
		        </ui:toolbar>

                <ui:scrollbox id="scrollbox">
                    Sent: <%= TimeStamp.ToString("dddd, dd MMMM yyyy, HH:mm:ss") %> <br /><br />
                    From: <%= Message.From.Address %> <br />
                    To: <%= String.Join(", ", Message.To.Select(o => o.Address)) %> <br />
                    Cc: <%= String.Join(", ", Message.CC.Select(o => o.Address)) %> <br />
                    Bcc: <%= String.Join(", ", Message.Bcc.Select(o => o.Address)) %> <br /><br />
                    <b><%: Message.Subject %></b> <br /> <br />

                    <asp:Repeater ID="rptAttachments" ItemType="CompositeC1Contrib.Email.Web.UI.MailAttachmentItem" runat="server">
                        <HeaderTemplate>
                            <ul>
                        </HeaderTemplate>
                        
                        <ItemTemplate>
                            <li>
                                <a href="view.aspx?view=<%= View %>&amp;id=<%= Id %>&amp;cmd=download&amp;attachmentId=<%# Item.Id %>" target="_blank">
                                    <%# Item.Name %> (<%# Item.Size %> bytes)
                                </a>
                            </li>
                        </ItemTemplate>

                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </asp:Repeater>

                    <br />
                    <%= Body %>
                </ui:scrollbox>
            </ui:page>
        </form>
    </body>
</html>
