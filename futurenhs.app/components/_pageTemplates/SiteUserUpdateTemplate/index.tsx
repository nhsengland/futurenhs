import { Props } from './interfaces'
import { actions as actionsConstants } from '@constants/actions'
import { selectForm } from '@selectors/forms'
import { FormConfig, FormErrors } from '@appTypes/form'
import { formTypes } from '@constants/forms'
import { useState } from 'react'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { Avatar } from '@components/Avatar'
import { initials } from '@helpers/formatters/initials'
import { FormWithErrorSummary } from '@components/FormWithErrorSummary'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { putSiteUser } from '@services/putSiteUser'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { getGenericFormError } from '@helpers/util/form'
import { useRouter } from 'next/router'
import { putSiteUserRole } from '@services/putSiteUserRole'

export const SiteUserUpdateTemplate: (props: Props) => JSX.Element = ({
    contentText,
    siteUser,
    actions,
    user,
    forms,
    csrfToken,
    routes,
    etag
}) => {

    const router = useRouter();

    const shouldRenderRoleForm: boolean = actions.includes(actionsConstants.SITE_ADMIN_MEMBERS_EDIT);

    const profileFormConfig: FormConfig = selectForm(forms, formTypes.UPDATE_SITE_USER);
    const roleFormConfig: FormConfig = selectForm(forms, formTypes.UPDATE_SITE_USER_ROLE);

    const [profileFormErrors, setProfileFormErrors] = useState(profileFormConfig.errors);
    const [roleFormErrors, setRoleFormErrors] = useState(profileFormConfig.errors);

    const siteUserInitials: string = initials({ value: `${siteUser.firstName} ${siteUser.lastName}` });

    const {
        editHeading,
        editRoleHeading
    } = contentText ?? {}

    /**
     * Handle client-side update submission for profile details
     */
    const handleProfileSubmit = async (formData: FormData): Promise<FormErrors> => {

        return new Promise((resolve) => {

            const etagToUse: string = typeof etag === 'object' ? etag.profileEtag : etag;

            const headers = getStandardServiceHeaders({ csrfToken, etag: etagToUse })

            putSiteUser({ body: formData, user, headers, targetUserId: siteUser.id })
                .then(() => {
                    setProfileFormErrors({})
                    resolve({})

                    router.replace(`${routes.usersRoot}/${siteUser.id}`)
                })
                .catch((error) => {
                    const errors: FormErrors =
                        getServiceErrorDataValidationErrors(error) ||
                        getGenericFormError(error)

                    setProfileFormErrors(errors)
                    resolve(errors)
                })
        })

    };

    /**
     * Handle client-side update submission for profile details
     */
    const handleRoleSubmit = async (formData: FormData): Promise<FormErrors> => {

        return new Promise((resolve) => {

            const etagToUse: string = typeof etag === 'object' ? etag.roleEtag : etag;
            
            const headers = getStandardServiceHeaders({ csrfToken, etag: etagToUse })

            putSiteUserRole({ body: formData, user, headers, targetUserId: siteUser.id })
                .then(() => {
                    setRoleFormErrors({})
                    resolve({})

                    router.replace(`${routes.usersRoot}/${siteUser.id}`)
                })
                .catch((error) => {
                    const errors: FormErrors =
                        getServiceErrorDataValidationErrors(error) ||
                        getGenericFormError(error)

                    setRoleFormErrors(errors)
                    resolve(errors)
                })
        })

    };


    /**
     * Render
     */
    return (
        <LayoutColumn className="c-page-body">
            <LayoutColumnContainer>
                <LayoutColumn tablet={6}>

                    <h1>{editHeading}</h1>
                    <Avatar
                        image={siteUser.image}
                        initials={siteUserInitials}
                        className="u-h-36 u-w-36 u-mb-8"
                    />
                    <FormWithErrorSummary
                        csrfToken={csrfToken}
                        formConfig={profileFormConfig}
                        errors={profileFormErrors}
                        text={{
                            form: {
                                submitButton: 'Save changes',
                                cancelButton: 'Discard changes'
                            }
                        }}
                        submitAction={handleProfileSubmit}
                        cancelHref={`${routes.usersRoot}/${siteUser.id}`}
                    />

                    {shouldRenderRoleForm &&
                        <>
                            <h2 className="u-mt-20">{editRoleHeading}</h2>
                            <FormWithErrorSummary
                                csrfToken={csrfToken}
                                formConfig={roleFormConfig}
                                errors={roleFormErrors}
                                text={{
                                    form: {
                                        submitButton: 'Update role'
                                    }
                                }}
                                submitAction={handleRoleSubmit}
                            />
                        </>
                    }

                </LayoutColumn>
            </LayoutColumnContainer>
        </LayoutColumn>
    )

}