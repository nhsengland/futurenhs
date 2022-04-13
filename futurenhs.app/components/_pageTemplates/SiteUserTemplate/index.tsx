import { useRouter } from 'next/router'
import { useState } from 'react'

import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { Avatar } from '@components/Avatar'
import { initials } from '@helpers/formatters/initials'
import { UserProfile } from '@components/UserProfile'
import { actions as actionsConstants } from '@constants/actions'
import { Link } from '@components/Link'
import { FormWithErrorSummary } from '@components/FormWithErrorSummary'
import { selectForm } from '@selectors/forms'
import { FormConfig, FormErrors } from '@appTypes/form'
import { getServiceErrorDataValidationErrors } from '@services/index';
import { getGenericFormError } from '@helpers/util/form';

import { Props } from './interfaces'
import { formTypes } from '@constants/forms'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { putSiteUser } from '@services/putSiteUser'


/**
 * Site user template
 */
export const SiteUserTemplate: (props: Props) => JSX.Element = ({
    contentText,
    siteUser,
    actions,
    user,
    forms,
    csrfToken,
    routes,
    etag
}) => {

    const router = useRouter()

    const formConfig: FormConfig = selectForm(forms, formTypes.UPDATE_SITE_USER);
    const [errors, setErrors] = useState(formConfig.errors);

    const {
        secondaryHeading,
        firstNameLabel,
        lastNameLabel,
        pronounsLabel,
        emailLabel,
        editHeading,
        editButtonLabel
    } = contentText ?? {}

    const hasEditPermissions: boolean = actions.includes(actionsConstants.SITE_ADMIN_MEMBERS_EDIT) || siteUser.id === user.id;
    const isEditMode: boolean = Boolean(router.query?.edit);
    const shouldRenderEditButton: boolean = hasEditPermissions && !isEditMode;

    const siteUserInitials: string = initials({ value: `${siteUser.firstName} ${siteUser.lastName}` });


    /**
     * Client-side submission handler
     */
    const handleSubmit = async (formData: FormData): Promise<FormErrors> => {

        return new Promise((resolve) => {
            const headers = getStandardServiceHeaders({ csrfToken, etag })
            
            putSiteUser({ siteUserId: siteUser.id, body: formData, user, headers })
                    .then(() => {
                        setErrors({})
                        resolve({})

                        router.replace(`${routes.usersRoot}/${siteUser.id}`)

                    })
                    .catch((error) => {
                        const errors: FormErrors =
                            getServiceErrorDataValidationErrors(error) ||
                            getGenericFormError(error)

                        setErrors(errors)
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
                        {isEditMode ?

                            hasEditPermissions &&
                            <>
                                <h1>{editHeading}</h1>
                                <Avatar
                                    image={siteUser.image}
                                    initials={siteUserInitials}
                                    className="u-h-36 u-w-36 u-mb-8"
                                />
                                <FormWithErrorSummary
                                    csrfToken={csrfToken}
                                    formConfig={formConfig}
                                    errors={errors}
                                    text={{
                                        form: {
                                            submitButton: 'Save changes',
                                            cancelButton: 'Discard changes'
                                        }
                                    }}
                                    submitAction={handleSubmit}
                                    cancelHref={`${routes.usersRoot}/${siteUser.id}`}
                                />
                            </>

                            :

                            <UserProfile
                                headingLevel={1}
                                profile={siteUser}
                                text={{
                                    heading: secondaryHeading,
                                    firstNameLabel: firstNameLabel,
                                    lastNameLabel: lastNameLabel,
                                    pronounsLabel: pronounsLabel,
                                    emailLabel: emailLabel,
                                }}
                                className="tablet:u-justify-center tablet:u-mt-16"
                            >
                                {shouldRenderEditButton &&
                                    <Link href={{
                                        pathname: `${routes.usersRoot}/${siteUser.id}`,
                                        query: {
                                            edit: 'true'
                                        }
                                    } as any}><a className="c-form_submit-button c-button u-w-full tablet:u-w-auto u-mt-8 c-button-outline">{editButtonLabel}</a></Link>
                                }
                            </UserProfile>



                        }
                    </LayoutColumn>
                </LayoutColumnContainer>
            </LayoutColumn>
    )
}
