﻿<?xml version="1.0" encoding="UTF-8" ?>

<%@ Page Language="C#" AutoEventWireup="true" Inherits="CompositeC1Contrib.Email.Web.UI.LogPage" %>
<%@ Import Namespace="CompositeC1Contrib.Email.C1Console" %>

<html xmlns="http://www.w3.org/1999/xhtml" xmlns:ui="http://www.w3.org/1999/xhtml" xmlns:control="http://www.composite.net/ns/uicontrol">
    <control:httpheaders runat="server" />

    <head runat="server">
        <title>Mail log</title>

        <control:styleloader runat="server" />
        <control:scriptloader type="sub" runat="server" />

        <script src="bindings/LogTableBinding.js"></script>
        <script src="bindings/LogCommandBinding.js"></script>

        <link href="logs.css" rel="stylesheet" />
    </head>

    <body
        id="root"
        data-consoleid="<%= ConsoleId %>"
        data-entitytoken="<%= EntityToken %>"
        data-view="<%= Request.QueryString["view"] %>"
        data-baseurl="<%= HttpUtility.HtmlAttributeEncode(BaseUrl) %>"
        data-template="<%= Request.QueryString["template"] %>"
        data-queue="<%= Request.QueryString["queue"] %>">
        <form runat="server" class="updateform updatezone">
            <ui:broadcasterset>
                <ui:broadcaster id="broadcasterHasSelection" isdisabled="true" />
            </ui:broadcasterset>

            <ui:popupset></ui:popupset>

            <ui:page id="mailLog" image="${icon:report}">
                <ui:toolbar id="toolbar">
                    <ui:toolbarbody>
                        <ui:toolbargroup>
                            <aspui:toolbarbutton runat="server"
                                autopostback="true" 
                                text="Refresh" 
                                imageurl="${icon:refresh}" 
                                onclick="OnRefresh" />
                            
                            <% if (View == QueueFolder.Queued || View == QueueFolder.BadMail) { %>
                                <aspui:toolbarbutton runat="server"
                                    autopostback="true" 
                                    text="Empty folder" 
                                    imageurl="${icon:delete}" 
                                    onclick="OnDeleteAll" />
                            <% } %>
                        </ui:toolbargroup>
                    </ui:toolbarbody>
                </ui:toolbar>

                <ui:flexbox id="flexbox">
                    <ui:scrollbox id="scrollbox">
                        <table id="filter">
                            <thead>
                                <tr>
                                    <th>From</th>
                                    <th class="input">
                                        <div class="datecontainer">
                                            <div>
                                                <ui:datainputbutton
                                                    callbackid="FromDateSelectorInput"
                                                    image="${icon:fields}" value="<%= FromDateWidget.SelectedDate.ToShortDateString() %>"
                                                    readonly="true" />
                                            </div>

                                            <div class="widget">
                                                <asp:PlaceHolder ID="FromDateWidgetPlaceHolder" Visible="False" runat="server">
                                                    <div class="calendar">
                                                        <asp:Calendar ID="FromDateWidget" runat="server" 
                                                            ShowDayHeader="true" 
                                                            OnSelectionChanged="CalendarSelectionChange"
                                                            OtherMonthDayStyle-CssClass="othermonth" 
                                                            SelectedDayStyle-CssClass="selectedday" />

                                                        <asp:LinkButton runat="server"
                                                            CssClass="calendaryearback" 
                                                            CommandName="Back" 
                                                            CommandArgument="From" 
                                                            OnClick="CalendarYearClick">back</asp:LinkButton>

                                                        <asp:LinkButton runat="server"
                                                            CssClass="calendaryearforward" 
                                                            CommandName="Forward" 
                                                            CommandArgument="From" 
                                                            OnClick="CalendarYearClick">forward</asp:LinkButton>
                                                    </div>
                                                </asp:PlaceHolder>
                                            </div>
                                        </div>
                                    </th>

                                    <th>To </th>
                                    <th class="input">
                                        <div class="datecontainer">
                                            <div>
                                                <ui:datainputbutton
                                                    callbackid="ToDateSelectorInput"
                                                    image="${icon:fields}" value="<%= ToDateWidget.SelectedDate.ToShortDateString() %>"
                                                    readonly="true" />
                                            </div>

                                            <div class="widget">
                                                <asp:PlaceHolder ID="ToDateWidgetPlaceHolder" Visible="False" runat="server">
                                                    <div class="calendar">
                                                        <asp:Calendar ID="ToDateWidget" runat="server" 
                                                            ShowDayHeader="true" 
                                                            OnSelectionChanged="CalendarSelectionChange"
                                                            OtherMonthDayStyle-CssClass="othermonth" 
                                                            SelectedDayStyle-CssClass="selectedday" />

                                                        <asp:LinkButton runat="server"
                                                            CssClass="calendaryearback" 
                                                            CommandName="Back" 
                                                            CommandArgument="To" 
                                                            OnClick="CalendarYearClick">back</asp:LinkButton>

                                                        <asp:LinkButton runat="server"
                                                            CssClass="calendaryearforward" 
                                                            CommandName="Forward" 
                                                            CommandArgument="To" 
                                                            OnClick="CalendarYearClick">forward</asp:LinkButton>
                                                    </div>
                                                </asp:PlaceHolder>
                                            </div>
                                        </div>
                                    </th>
                                
                                    <th>Template</th>

                                    <th>
                                        <aspui:selector runat="server" id="ddlTemplates" autopostback="True" onselectedindexchanged="OnTemplateChanged" datavaluefield="Key" datatextfield="Value" />
                                    </th>

                                    <th width="100%"></th>
                                </tr>
                            </thead>
                        </table>

                        <asp:Repeater ID="rptLog" ItemType="CompositeC1Contrib.Email.Web.UI.MailLogItem" runat="server">
                            <HeaderTemplate>
                                <table width="100%" id="logtable" binding="LogTableBinding">
                                    <thead>
                                        <tr>
                                            <th>
                                                <ui:text label="Subject" />
                                            </th>

                                            <th width="100px">
                                                <ui:text label="Timestamp" />
                                            </th>

                                            <th width="350px">
                                                <ui:text label="Template" />
                                            </th>
                                        
                                            <% if (View == QueueFolder.BadMail) { %>
                                                <th width="75px"></th>
                                            <% } %>

                                            <% if (View == QueueFolder.Queued || View == QueueFolder.BadMail) { %>
                                                <th width="75px"></th>
                                            <% } %>

                                            <th width="75px"></th>
                                        </tr>
                                    </thead>

                                    <tbody>
                            </HeaderTemplate>

                            <ItemTemplate>
                                        <tr>
                                            <td>
                                                <ui:labelbox label="<%# Server.HtmlEncode(Item.Subject) %>" />
                                            </td>

                                            <td>
                                                <ui:labelbox label="<%# Item.FormatTimeStamp() %>" />
                                            </td>

                                            <td>
                                                <ui:labelbox label="<%# Item.Template == null ? "No template" : Server.HtmlEncode(Item.Template.Key) %>" />
                                            </td>
                                
                                            <% if (View == QueueFolder.BadMail) { %>
                                                <td class="command">
                                                    <ui:labelbox label="Requeue" image="${icon:accept}"
                                                        binding="LogCommandBinding"
                                                        link="log.aspx<%= BaseUrl.Replace("&", "&amp;") %>&amp;cmd=requeue&amp;id=<%# Item.Id %>" class="requeue" data-id="<%# Item.Id %>" />
                                                </td>
                                            <% } %>

                                            <% if (View == QueueFolder.Queued || View == QueueFolder.BadMail) { %>
                                                <td class="command">
                                                    <ui:labelbox label="Delete" image="${icon:delete}"
                                                        binding="LogCommandBinding"
                                                        link="log.aspx<%= BaseUrl.Replace("&", "&amp;") %>&amp;cmd=delete&amp;id=<%# Item.Id %>" class="delete" data-id="<%# Item.Id %>" />
                                                </td>
                                            <% } %>

                                            <td class="command">
                                                <ui:labelbox label="View" image="${icon:log}"
                                                    binding="LogCommandBinding"
                                                    link="view.aspx<%= BaseUrl.Replace("&", "&amp;") %>&amp;id=<%# Item.Id %>" />
                                            </td>
                                        </tr>
                            </ItemTemplate>

                            <FooterTemplate>
                                    </tbody>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </ui:scrollbox>

                    <ui:toolbar id="bottomtoolbar">
                        <ui:toolbarbody>
                            <ui:toolbargroup>
                                <aspui:selector runat="server" id="PageSize" autopostback="true" onselectedindexchanged="OnRefresh">
                                    <asp:ListItem>25</asp:ListItem>
                                    <asp:ListItem>100</asp:ListItem>
                                    <asp:ListItem>500</asp:ListItem>
                                </aspui:selector>
                            </ui:toolbargroup>

                            <ui:toolbargroup>
                                <aspui:toolbarbutton runat="server"
                                    id="PrevPage"
                                    client_tooltip="Previous"
                                    client_image="${icon:previous}"
                                    client_image-disabled="${icon:previous-disabled}"
                                    onclick="Prev" />

                                <aspui:textbox runat="server" readonly="True" id="PageNumber" width="20" />

                                <aspui:toolbarbutton runat="server"
                                    id="NextPage"
                                    client_tooltip="Next"
                                    client_image="${icon:next}"
                                    client_image-disabled="${icon:next-disabled}"
                                    onclick="Next" />
                            </ui:toolbargroup>
                        </ui:toolbarbody>
                    </ui:toolbar>

                    <ui:cover id="cover" hidden="true" />
                </ui:flexbox>
            </ui:page>
        </form>
    </body>
</html>
