import { LayoutColumn } from '@components/LayoutColumn'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { Props } from './interfaces'

/**
 * Group member invite template
 */
export const GroupMemberInviteTemplate: (props: Props) => JSX.Element = ({
    csrfToken,
    forms,
    routes,
    user,
    contentText,
}) => {
    const { secondaryHeading } = contentText

    /**
     * Render
     */
    return (
        <LayoutColumn className="c-page-body">
            <LayoutColumnContainer>
                <LayoutColumn tablet={8}>
                    <h2>{secondaryHeading}</h2>
                </LayoutColumn>
            </LayoutColumnContainer>
        </LayoutColumn>
    )
}
