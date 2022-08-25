import { Props } from './interfaces'
import { actions as actionsConstants } from '@constants/actions'
import { selectForm, selectFormErrors } from '@selectors/forms'
import { FormConfig, FormErrors } from '@appTypes/form'
import { formTypes } from '@constants/forms'
import { useRef, useState } from 'react'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { Avatar } from '@components/Avatar'
import { initials } from '@helpers/formatters/initials'
import { Form } from '@components/Form'
import { ErrorSummary } from '@components/ErrorSummary'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { postRegisterSiteUser } from '@services/postRegisterSiteUser'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { getGenericFormError } from '@helpers/util/form'
import { useRouter } from 'next/router'
import { useFormConfig } from '@hooks/useForm'
import { PageBody } from '@components/PageBody'
import { RichText } from '@components/RichText'

export const AuthRegisterTemplate: (props: Props) => JSX.Element = ({
    contentText,
    forms,
    csrfToken,
    routes,
    etag,
    subjectId,
    emailAddress,
    issuer,
}) => {
    const router = useRouter()
    const errorSummaryRef: any = useRef()

    //const { authSignOut } = routes;
    const profileFormConfig: FormConfig = useFormConfig(
        formTypes.REGISTER_SITE_USER,
        forms[formTypes.REGISTER_SITE_USER]
    )

    const [errors, setErrors] = useState(
        Object.assign({}, selectFormErrors(forms, formTypes.REGISTER_SITE_USER))
    )

    const { mainHeading, bodyHtml } = contentText ?? {}

    /**
     * Handle client-side validation failure in forms
     */
    const handleValidationFailure = (errors: FormErrors): void => {
        setErrors(errors)
        errorSummaryRef?.current?.focus?.()
    }

    /**
     * Handle client-side update submission for profile details
     */
    const handleProfileSubmit = async (
        formData: FormData
    ): Promise<FormErrors> => {
        return new Promise((resolve) => {
            const etagToUse: string =
                typeof etag === 'object' ? etag.profileEtag : etag

            const headers = getStandardServiceHeaders({
                csrfToken,
                etag: etagToUse,
            })

            postRegisterSiteUser({
                body: formData,
                headers,
                subjectId: subjectId,
                emailAddress: emailAddress,
                issuer: issuer,
            })
                .then(() => {
                    router.push(routes.groupsRoot)
                    setErrors({})
                    resolve({})
                })
                .catch((error) => {
                    const errors: FormErrors =
                        getServiceErrorDataValidationErrors(error) ||
                        getGenericFormError(error)

                    setErrors(errors)
                    resolve(errors)
                })
        })
    }

    /**
     * Render
     */
    return (
        <PageBody className="tablet:u-px-0">
            <LayoutColumnContainer justify="centre">
                <LayoutColumn tablet={8} desktop={6}>
                    <h1 className="nhsuk-heading-xl">{mainHeading}</h1>
                    <RichText bodyHtml={bodyHtml} />
                    <ErrorSummary
                        ref={errorSummaryRef}
                        errors={errors}
                        className="u-mb-10"
                    />
                    <Form
                        csrfToken={csrfToken}
                        formConfig={profileFormConfig}
                        text={{
                            submitButton: 'Save changes',
                            cancelButton: 'Discard changes',
                        }}
                        submitAction={handleProfileSubmit}
                        cancelHref={`signout`}
                        validationFailAction={handleValidationFailure}
                    />
                </LayoutColumn>
            </LayoutColumnContainer>
        </PageBody>
    )
}
