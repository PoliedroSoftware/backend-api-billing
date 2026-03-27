
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poliedro.Billing.Domain.AuthPlemsi.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poliedro.Billing.Infrastructure.Persistence.Mysql.EntityFramework.EntityConfigurations;

public class AuthPlemsiClientConfiguration  
{
    public AuthPlemsiClientConfiguration(EntityTypeBuilder<AuthPlemsiClient> entity)
    {
        entity.ToTable("auth_plemsi_client");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(e => e.UserName).HasColumnName("user_name").HasMaxLength(255);
        entity.Property(e => e.UserAccess).HasColumnName("user_access").HasMaxLength(100);
        entity.Property(e => e.KeyAccess).HasColumnName("key_access").HasMaxLength(500);
        entity.Property(e => e.ResolutionId).HasColumnName("resolution_id");
        entity.Property(e => e.CreditNoteId).HasColumnName("credit_note_id");
        entity.Property(e => e.MultipleResolution).HasColumnName("multiple_resolution");
        entity.Property(e => e.HeadNote).HasColumnName("head_note");
        entity.Property(e => e.FoodNote).HasColumnName("food_note");
        entity.Property(e => e.Active).HasColumnName("active");
    }
}