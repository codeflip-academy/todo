﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Todo.Core;
using Todo.Domain;

namespace Todo.Domain
{
    public abstract class AccountsLists : Entity
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid? ListId { get; set; }
        public byte Role { get; protected set; }
        public bool UserIsOwner(Guid accountId) => AccountId == accountId && Role == Roles.Owner;
        public bool UserIsAlreadyInvited(Guid accountId)
        {
            if (Role == Roles.Invited)
            {
                return true;
            }
            return false;
        }

        public void MoveListToTrash()
        {
            var listId = this.ListId;

            this.ListId = null;
        }
        private protected AccountsLists() { }
    }

    public class RoleInvited : AccountsLists
    {
        public void Invited()
        {
            base.Role = Roles.Invited;
            DomainEvents.Add(new InvitationSent() { ListId = this.ListId, InviteeAccountId = this.AccountId });
        }
        public void MakeContributor()
        {
            base.Role = Roles.Contributor;
        }
        public void MakeDeclined()
        {
            base.Role = Roles.Declined;
        }
    }

    public class RoleOwner : AccountsLists
    {
        public void Owned()
        {
            base.Role = Roles.Owner;
        }
    }

    public class RoleContributor : AccountsLists
    {
        public void Contributed()
        {
            base.Role = Roles.Contributor;
        }
        public void MakeLeft()
        {
            base.Role = Roles.Left;
            DomainEvents.Add(new ContributorLeft() { ListId = this.ListId, AccountId = this.AccountId });
        }
    }

    public class RoleDecline : AccountsLists
    {
        public void Decline()
        {
            base.Role = Roles.Declined;
        }
        public void MakeInvited()
        {
            base.Role = Roles.Invited;
            DomainEvents.Add(new InvitationSent() { ListId = this.ListId, InviteeAccountId = this.AccountId });
        }
    }
    public class RoleLeft : AccountsLists
    {
        public void Left()
        {
            base.Role = Roles.Left;
        }
        public void MakeInvited()
        {
            base.Role = Roles.Invited;
            DomainEvents.Add(new InvitationSent() { ListId = this.ListId, InviteeAccountId = this.AccountId });
        }
    }
}
