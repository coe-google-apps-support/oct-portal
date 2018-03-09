using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoE.Ideas.Core.Data
{
    public class InitiativeInfo
    {
        public Guid Id { get; set; }
        public int AlternateKey { get; set; }

        /// <summary>
        /// The short title of the idea
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Title { get; private set; }

        /// <summary>
        /// The long description of the idea, can be HTML formatted
        /// </summary>
        [Required]
        public string Description { get; private set; }


        /// <summary>
        /// Sets/Returns the identity of the person or process that created the item.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The identity contained within this property is dependent upon the authentication method used by the application.
        /// </para>
        /// <para>
        /// <note type="note">This value is managed by the framework and its value is immutable.</note>
        /// </para>
        /// </remarks>
        [StringLength(128)]
        [MaxLength(128)]
        [Display(Description = "Contains the identity of the person or process that created the item.", Name = "Created By")]
        public string AuditCreatedBy { get; private set; }

        /// <summary>
        /// Sets/Returns the date and time that the item was created.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The date and time value is in the <see cref="DateTimeOffset"/> format.
        /// </para>
        /// <para>
        /// <note type="note">This value is managed by the framework and its value is immutable.</note>
        /// </para>
        /// </remarks>
        [Display(Description = "Contains the creation date and time for the item.", Name = "Created On")]
        public DateTimeOffset? AuditCreatedOn { get; private set; }

        /// <summary>
        /// Sets/Returns the identity of the person or process that last updated the item.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The identity contained within this property is dependent upon the authentication method used by the application.
        /// </para>
        /// <para>
        /// <note type="note">This value is managed by the framework and its value is immutable.</note>
        /// </para>
        /// </remarks>
        [StringLength(128)]
        [MaxLength(128)]
        [Display(Description = "Contains the identity of the person or process that last updated the item.", Name = "Updated By")]
        public string AuditUpdatedBy { get; protected set; }

        /// <summary>
        /// Sets/Returns the date and time that the item was last updated.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The date and time value is in the <see cref="DateTimeOffset"/> format.
        /// </para>
        /// <para>
        /// <note type="note">This value is managed by the framework and its value is immutable.</note>
        /// </para>
        /// </remarks>
        [Display(Description = "Contains the date and time when the item was last updated.", Name = "Updated On")]
        public DateTimeOffset? AuditUpdatedOn { get; private set; }
    }
}
