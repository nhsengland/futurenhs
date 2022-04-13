import { GetServerSideProps } from 'next'

import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { routeParams } from '@constants/routes'
import { withUser } from '@hofs/withUser'
import { withTextContent } from '@hofs/withTextContent'
import { withRoutes } from '@hofs/withRoutes'
import { selectCsrfToken, selectFormData, selectMultiPartFormData, selectParam, selectRequestMethod } from '@selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { getSiteUser } from '@services/getSiteUser'
import { formTypes } from '@constants/forms'

import { SiteUserTemplate } from '@components/_pageTemplates/SiteUserTemplate'
import { Props } from '@components/_pageTemplates/SiteUserTemplate/interfaces'
import { withForms } from '@hofs/withForms'
import { requestMethods } from '@constants/fetch'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { putSiteUser } from '@services/putSiteUser'
import { FormErrors } from '@appTypes/form'
import { getServiceErrorDataValidationErrors } from '@services/index'

const routeId: string = '9e86c5cc-6836-4319-8d9d-b96249d4c909'
const props: Partial<Props> = {}

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withUser({
    props,
    getServerSideProps: withRoutes({
        props,
        getServerSideProps: withForms({
            props,
            getServerSideProps: withTextContent({
                props,
                routeId,
                getServerSideProps: async (context: GetServerSidePropsContext) => {
                    const userId: string = selectParam(context, routeParams.USERID)

                    const csrfToken: string = selectCsrfToken(context)
                    const currentValues: any = selectFormData(context)
                    const submission: any = selectMultiPartFormData(context)
                    const requestMethod: requestMethods = selectRequestMethod(context)

                    const form: any =
                        props.forms[formTypes.UPDATE_SITE_USER]

                    /**
                     * Get data from services
                     */
                    try {
                        const [userData] = await Promise.all([
                            getSiteUser({ userId }),
                        ])

                        const etag = userData.headers.get('etag');
                        props.etag = etag;

                        props.siteUser = userData.data
                        props.pageTitle = `${props.contentText.title} - ${props.siteUser.firstName ?? ''
                            } ${props.siteUser.lastName ?? ''}`

                        form.initialValues = {
                            firstName: props.siteUser.firstName,
                            lastName: props.siteUser.lastName,
                            pronouns: props.siteUser.pronouns,
                            image: props.siteUser.image
                        }

                        /**
                         * Handle server-side form post
                         */
                        if (
                            submission &&
                            requestMethod === requestMethods.POST
                        ) {

                            form.initialValues = currentValues.getAll();

                            const headers = {
                                ...getStandardServiceHeaders({
                                    csrfToken,
                                    etag,
                                }),
                                ...submission.getHeaders(),
                            }

                            await putSiteUser({
                                siteUserId: userId,
                                headers,
                                body: submission,
                            })

                            return {
                                redirect: {
                                    permanent: false,
                                    destination: `${props.routes.usersRoot}/${userId}`,
                                },
                            }
                        }
                    } catch (error: any) {

                        const validationErrors: FormErrors =
                            getServiceErrorDataValidationErrors(error)

                        if (validationErrors) {
                            form.errors = validationErrors
                        } else {
                            return handleSSRErrorProps({ props, error })
                        }
                    }

                    /**
                     * Return data to page template
                     */
                     return handleSSRSuccessProps({ props })
            },
            })
    }),
}),
})

/**
 * Export page template
 */
export default SiteUserTemplate
