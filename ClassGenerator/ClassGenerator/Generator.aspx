<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Generator.aspx.cs" Inherits="ClassGenerator.Generator" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="description" content="">
    <meta name="author" content="">

    <title>Starter Template for Bootstrap</title>

    <!-- Bootstrap core CSS -->
    <link href="/Content/bootstrap.min.css" rel="stylesheet">

    <!-- HTML5 shim and Respond.js for IE8 support of HTML5 elements and media queries -->
    <!--[if lt IE 9]>
      <script src="https://oss.maxcdn.com/html5shiv/3.7.2/html5shiv.min.js"></script>
      <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script>
    <![endif]-->
</head>
<body>
    <div class="container">
        <h1>GENERATOR BY MILO</h1>
        <form id="form1" runat="server">

            <div class="row">
                <div class="col-md-12">
                    <div class="form-group">
                        <label for="txtClassName">Class Name</label>
                        <asp:TextBox ID="txtClassName" runat="server"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label for="txtTableName">Table Name</label>
                        <asp:TextBox ID="txtTableName" runat="server"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label for="txtSPPrefix">Store Procedure Prefix</label>
                        <asp:TextBox ID="txtSPPrefix" runat="server"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label for="txtField">Enter field according to sample format</label>
                        <asp:TextBox ID="txtField" runat="server" TextMode="MultiLine" Rows="10"></asp:TextBox>
                    </div>

                    <asp:Button ID="btnGenerator" runat="server" Text="Generate >>" OnClick="btnGenerator_Click" CssClass="btn btn-default" />
                </div>
            </div>

            <div class="row">
                <div class="col-md-12">
                    <div class="form-group">
                        <label for="txtClassCode">Generated Class</label>
                        <asp:TextBox ID="txtClassCode" runat="server" TextMode="MultiLine" Rows="10" Width="100%"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label for="txtStoredProcedure">Generated Stored Procedure</label>
                        <asp:TextBox ID="txtStoredProcedure" runat="server" TextMode="MultiLine" Rows="10" Width="100%"></asp:TextBox>
                    </div>
                    
                    <div class="form-group">
                        <label for="txtClassCodeInlineSQL">Generated Class with inline SQL (No Stored Procedure)</label>
                        <asp:TextBox ID="txtClassCodeInlineSQL" runat="server" TextMode="MultiLine" Rows="10" Width="100%"></asp:TextBox>
                    </div>
                </div>
            </div>
        </form>
    </div>


    <!-- Bootstrap core JavaScript
    ================================================== -->
    <!-- Placed at the end of the document so the pages load faster -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
    <script src="/Scripts/bootstrap.min.js"></script>
</body>
</html>
