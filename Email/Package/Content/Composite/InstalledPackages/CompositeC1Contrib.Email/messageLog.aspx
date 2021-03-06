﻿<?xml version="1.0" encoding="UTF-8" ?>

<%@ Page Language="C#" AutoEventWireup="true" Inherits="CompositeC1Contrib.Email.Web.UI.MessageLogPage" %>

<html xmlns="http://www.w3.org/1999/xhtml" xmlns:ui="http://www.w3.org/1999/xhtml" xmlns:control="http://www.composite.net/ns/uicontrol">
    <control:httpheaders runat="server" />

    <head runat="server">
        <title>Message log</title>

        <control:styleloader runat="server" />
        <control:scriptloader type="sub" runat="server" />

        <link href="logs.css" rel="stylesheet" />
    </head>

    <body>
        <form runat="server">
            <ui:page id="mailLog" image="${icon:report}">
                <ui:toolbar id="toolbar">
                    <ui:toolbarbody>
                        <ui:toolbargroup>
                            <aspui:toolbarbutton autopostback="true" text="Back" imageurl="${icon:back}" runat="server" onclick="OnBack" />
                            <aspui:toolbarbutton autopostback="true" text="Refresh" imageurl="${icon:refresh}" runat="server" onclick="OnRefresh" />
                        </ui:toolbargroup>
                    </ui:toolbarbody>
                </ui:toolbar>

                <ui:scrollbox id="scrollbox">
                    <table width="100%" id="logtable">
                        <asp:Repeater ID="rptLogColumns" runat="server">
                            <HeaderTemplate>
                                <thead>
                                    <tr>
                            </HeaderTemplate>

                            <ItemTemplate>
                                <th>
                                    <ui:text label="<%# Server.HtmlEncode((string)Container.DataItem) %>" />
                                </th>
                            </ItemTemplate>

                            <FooterTemplate>
                                </tr>
                                        </thead>
                            </FooterTemplate>
                        </asp:Repeater>

                        <asp:Repeater ID="rptLog" ItemType="CompositeC1Contrib.Email.Data.Types.IEvent" runat="server">
                            <HeaderTemplate>
                                <tbody>
                            </HeaderTemplate>

                            <ItemTemplate>
                                <tr>
                                    <asp:Repeater DataSource="<%# GetColumnsForLogItems(Item) %>" runat="server">
                                        <ItemTemplate>
                                            <td>
                                                <ui:labelbox label="<%# Server.HtmlEncode((string)Container.DataItem) %>" />
                                            </td>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tr>
                            </ItemTemplate>

                            <FooterTemplate>
                                </tbody>
                            </FooterTemplate>
                        </asp:Repeater>
                    </table>
                </ui:scrollbox>
            </ui:page>
        </form>
    </body>
</html>