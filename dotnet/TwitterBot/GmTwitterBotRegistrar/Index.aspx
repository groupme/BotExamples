<%@ Page Language="C#" Async="true" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="GmTwitterBotRegistrar.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>GroupMe TwitterBot Manager</title>
</head>
<body>
    <form id="form1" runat="server" style="margin-top:20px;margin-left:12px">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <asp:UpdatePanel ID="updatePanel" runat="server">
            <ContentTemplate>
                <asp:Panel ID="LogInPanel" runat="server">
                    <p>
                        Welcome to TwitterBot Manager! Log in with your GroupMe account to get started #)
                    </p>
            <asp:Button ID="Submit" runat="server" Text="Connect" OnClick="Submit_Click" />
                    <p>
                        <asp:Label ID="ErrorLabel" runat="server" Visible="false" />
                    </p>
                </asp:Panel>
                <asp:Panel ID="MainPanel" runat="server" Visible="false">
                    <p>
                        <asp:Label ID="NameLabel" runat="server" />
                    </p>
                    <p><strong>Manage existing bots</strong></p>
                    <asp:DataGrid ID="ExistingBots" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" >
                        <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                        <EditItemStyle BackColor="#999999" />
                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                        <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                    </asp:DataGrid>
                    <p><strong>Create a new bot</strong></p>
                    <table>
                        <tr>
                            <td>Choose a group: </td>
                            <td><asp:DropDownList ID="GroupList" runat="server" DataTextField="GroupName" DataValueField="Id" /></td>
                        </tr>
                        <tr>
                            <td>Twitter search term: </td>
                            <td><asp:TextBox ID="TwitterSearchTerm" runat="server" /></td>
                        </tr>
                    </table>
                    <p>
                        <asp:Button ID="CreateButton" runat="server" OnClick="CreateButton_Click" Text="Create TwitterBot" />
                    </p>
                    <p>
                        <asp:Label ID="Result" runat="server" />
                    </p>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
