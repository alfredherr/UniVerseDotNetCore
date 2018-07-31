﻿using System;
using IBMU2.UODOTNET;
using UniVerseDotNetCore.Domain.CssServiceLayer.Models;
using UniVerseDotNetCore.Domain.Models;
using UniVerseDotNetCore.Models;

namespace UniVerseDotNetCore.Domain.CssServiceLayer
{
    public static class ChangeItCapability
    {
        public static CssCommandResult ChangeLenderCode(CssAccountFile file, AccountList list, LenderCode newCode, Note changeNote, CssCredentialsModel cssCredentialsModel)
        {
            return ChangeIt2(file, list, "", newCode.Code, "", changeNote, cssCredentialsModel);
        }
        public static CssCommandResult ChangeProjectCode(CssAccountFile file, AccountList list, ProjectCode newCode, Note changeNote, CssCredentialsModel cssCredentialsModel)
        {
            return ChangeIt2(file, list, newCode.Code, "", "", changeNote, cssCredentialsModel);
        }
        public static CssCommandResult ChangeAccountCode(CssAccountFile file, AccountList list, AccountCode newCode, Note changeNote, CssCredentialsModel cssCredentialsModel)
        {
            return ChangeIt2(file, list, "", "", newCode.Code, changeNote, cssCredentialsModel);
        }
        private static CssCommandResult ChangeIt2(CssAccountFile file, AccountList list, string projectCode, string lenderCode, string accountCode, Note changeNote, CssCredentialsModel cssCredentialsModel)
        {
            var result = new CssCommandResult();
            var lHostName = cssCredentialsModel.Hostname;
            var lAccount = cssCredentialsModel.Account;
            var lUser = cssCredentialsModel.User;
            var lPassword = cssCredentialsModel.UserPassword;
            var lServiceType = cssCredentialsModel.ServiceType;
            string fileInitial = file.FileName.ToCharArray()[0].ToString();
            UniSession us = null;

            try
            {
                //get the session object
                us = UniObjects.OpenSession(lHostName, lUser, lPassword, lAccount, lServiceType);

                UniCommand cmd = us.CreateUniCommand();
                cmd.Command = "CHANGE.IT2";
                cmd.Execute();
                //result.Results.Add(new CommandResponse(cmd.Command, cmd.Response));

                //ENTER THE LIST NAME YOU ARE USING 
                cmd.Reply($"{list.AccountListName}");
                //result.Results.Add(new CommandResponse("", cmd.Response));

                //'C'ONTRACT OR 'M'AINTENANCE IDS? ?
                cmd.Reply($"{fileInitial}");
                //result.Results.Add(new CommandResponse($"{fileInitial}", cmd.Response));

                //CHANGE MAINTENANCE TOO? (IF APPLICABLE) ? 
                cmd.Reply("N");
                //result.Results.Add(new CommandResponse("N", cmd.Response));

                //ENTER NEW DEVELOPER CODE, IF ANY ?
                cmd.Reply("");
                //result.Results.Add(new CommandResponse("Developer", cmd.Response));

                //Project?
                cmd.Reply($"{projectCode}");
                //result.Results.Add(new CommandResponse($"{projectCode}", cmd.Response));

                //Lender
                cmd.Reply($"{lenderCode}");
                //result.Results.Add(new CommandResponse($"{lenderCode}", cmd.Response));

                //AccountCode
                cmd.Reply($"{accountCode}");
                //result.Results.Add(new CommandResponse($"{accountCode}", cmd.Response));

                //Note
                cmd.Reply($"{changeNote.Message}");
                //result.Results.Add(new CommandResponse($"{changeNote.Message}", cmd.Response));


                //Note Type
                cmd.Reply("G");
                //result.Results.Add(new CommandResponse($"NoteType", cmd.Response));

                if (cmd.Response.Contains("OKAY TO PROCEED? (Y/N)"))
                {
                    //OKAY TO PROCEED? (Y/N) 
                    cmd.Reply("Y");
                }
                else
                {
                    throw new Exception($"Could not get confirmation message. Raw result: {cmd.Response}");
                }

                int lineNumber = 0;
                foreach (var line in cmd.Response.Split("\r\n"))
                {
                    result.Results.Add(new CommandResponse($"Response Line: {++lineNumber}", line));

                }

                return result;

            }
            catch (Exception ex)
            {
                result.Results.Add(new CommandResponse("Exception", ex.ToString()));
            }
            finally
            {
                if (us != null && us.IsActive)
                {
                    UniObjects.CloseSession(us);
                }
            }

            result.Results.Add(new CommandResponse("Error", "There was an error procesing your request."));
            return result;
        }

    }
}