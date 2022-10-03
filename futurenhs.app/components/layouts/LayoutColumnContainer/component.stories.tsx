import { LayoutColumnContainer } from './index'
import { LayoutColumn } from '../LayoutColumn'

export default {
    title: 'LayoutColumnContainer',
    component: LayoutColumnContainer,
}

const Template = (args) => (
    <div className="u-p-14">
        <LayoutColumnContainer {...args}>
            <LayoutColumn tablet={3} className="u-bg-theme-3 u-max-h-[100px]">
                <p className="u-h-14 u-p-4">Column 1</p>
            </LayoutColumn>
            <LayoutColumn tablet={3} className="u-bg-theme-3 u-max-h-[100px]">
                <p className="u-h-14 u-p-4">Column 2</p>
            </LayoutColumn>
        </LayoutColumnContainer>
    </div>
)

const smallExampleContent: string =
    'Lorem ipsum dolor sit amet, consectetur adipiscing eli'
const largeExampleContent: string =
    'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.'

const TemplateGrid = (args) => (
    <div className="u-p-14">
        <LayoutColumnContainer {...args}>
            <LayoutColumn
                tablet={6}
                className="u-mb-6 u-bg-theme-3 u-min-h-[100px]"
            >
                <div className="u-p-4">
                    <p>Column 1</p>
                    <p>{largeExampleContent}</p>
                </div>
            </LayoutColumn>
            <LayoutColumn
                tablet={6}
                className="u-mb-6 u-bg-theme-3 u-min-h-[100px]"
            >
                <div className="u-p-4">
                    <p>Column 2</p>
                    <p>{smallExampleContent}</p>
                </div>
            </LayoutColumn>
            <LayoutColumn
                tablet={12}
                className="u-mb-6 u-bg-theme-3 u-min-h-[100px]"
            >
                <div className="u-p-4">
                    <p>Column 3</p>
                    <p>{smallExampleContent}</p>
                </div>
            </LayoutColumn>
            <LayoutColumn
                tablet={4}
                className="u-mb-6 u-bg-theme-3 u-min-h-[100px]"
            >
                <div className="u-p-4">
                    <p>Column 4</p>
                    <p>{smallExampleContent}</p>
                </div>
            </LayoutColumn>
            <LayoutColumn
                tablet={4}
                className="u-mb-6 u-bg-theme-3 u-min-h-[100px]"
            >
                <div className="u-p-4">
                    <p>Column 5</p>
                    <p>{largeExampleContent}</p>
                </div>
            </LayoutColumn>
            <LayoutColumn
                tablet={4}
                className="u-mb-6 u-bg-theme-3 u-min-h-[100px]"
            >
                <div className="u-p-4">
                    <p>Column 6</p>
                    <p>{largeExampleContent}</p>
                </div>
            </LayoutColumn>
            <LayoutColumn
                tablet={8}
                className="u-mb-6 u-bg-theme-3 u-min-h-[100px]"
            >
                <div className="u-p-4">
                    <p>Column 7</p>
                    <p>{smallExampleContent}</p>
                </div>
            </LayoutColumn>
            <LayoutColumn
                tablet={4}
                className="u-mb-6 u-bg-theme-3 u-min-h-[100px]"
            >
                <div className="u-p-4">
                    <p>Column 8</p>
                    <p>{smallExampleContent}</p>
                </div>
            </LayoutColumn>
            <LayoutColumn
                tablet={10}
                className="u-mb-6 u-bg-theme-3 u-min-h-[100px]"
            >
                <div className="u-p-4">
                    <p>Column 9</p>
                    <p>{smallExampleContent}</p>
                </div>
            </LayoutColumn>
            <LayoutColumn
                tablet={3}
                className="u-mb-6 u-bg-theme-3 u-min-h-[100px]"
            >
                <div className="u-p-4">
                    <p>Column 10</p>
                    <p>{smallExampleContent}</p>
                </div>
            </LayoutColumn>
            <LayoutColumn
                tablet={9}
                className="u-mb-6 u-bg-theme-3 u-min-h-[100px]"
            >
                <div className="u-p-4">
                    <p>Column 11</p>
                    <p>{smallExampleContent}</p>
                </div>
            </LayoutColumn>
        </LayoutColumnContainer>
    </div>
)

export const Basic = Template.bind({})
Basic.args = {
    className: 'u-h-[300px] u-border-dashed u-border-4',
}

export const Grid = TemplateGrid.bind({})
Grid.args = {
    className: 'u-border-dashed u-border-4',
}
